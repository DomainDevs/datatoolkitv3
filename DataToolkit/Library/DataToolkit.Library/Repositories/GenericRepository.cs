using Dapper;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using DataToolkit.Library.Engine.Abstractions;
using System.Data;
using DataToolkit.Library.Common.Metadata;

namespace DataToolkit.Library.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ISqlExecutor _sql;
    private readonly EntityMetadata _meta;

    private static readonly ConcurrentDictionary<Type, bool> _typeMapCache = new();

    public GenericRepository(ISqlExecutor sqlExecutor)
    {
        _sql = sqlExecutor;
        _meta = EntityMetadataHelper.GetMetadata<T>();

        EnsureDapperTypeMap();
    }

    // =========================================================
    // CORE: MAPPING (NO CAMBIA TU DISEÑO)
    // =========================================================

    private void EnsureDapperTypeMap()
    {
        if (_typeMapCache.ContainsKey(typeof(T)))
            return;

        SqlMapper.SetTypeMap(
            typeof(T),
            new CustomPropertyTypeMap(
                typeof(T),
                (type, columnName) =>
                {
                    var prop = _meta.ColumnMappings
                        .FirstOrDefault(x => x.Value.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                        .Key;

                    return prop ?? type.GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                })
        );

        _typeMapCache[typeof(T)] = true;
    }

    // =========================================================
    // READ
    // =========================================================

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? selectProperties)
    {
        var sql = $"SELECT {BuildColumns(selectProperties)} FROM {_meta.TableName}";
        return await _sql.FromSqlAsync<T>(sql);
    }

    public async Task<T?> GetByIdAsync(T entity, params Expression<Func<T, object>>[]? selectProperties)
    {
        var sql = $"SELECT {BuildColumns(selectProperties)} FROM {_meta.TableName} WHERE {BuildWhere()}";

        var result = await _sql.FromSqlAsync<T>(sql, entity);
        return result.FirstOrDefault();
    }

    // =========================================================
    // WRITE
    // =========================================================

    public Task<int> InsertAsync(T entity)
        => _sql.ExecuteAsync(BuildInsert(), entity);

    public Task<int> UpdateAsync(T entity, params Expression<Func<T, object>>[] includeProperties)
        => _sql.ExecuteAsync(BuildUpdate(includeProperties), entity);

    public Task<int> DeleteAsync(T entity)
        => _sql.ExecuteAsync($"DELETE FROM {_meta.TableName} WHERE {BuildWhere()}", entity);

    // =========================================================
    // STORED PROCEDURES
    // =========================================================

    public Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string sp, object parameters)
        => _sql.FromSqlAsync<T>(sp, parameters);

    public Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string sp, object parameters)
        => _sql.FromSqlAsync<TResult>(sp, parameters);

    // =========================================================
    // METADATA HELPERS (TU ESTILO ORIGINAL SE MANTIENE)
    // =========================================================

    private string BuildColumns(params Expression<Func<T, object>>[]? selectProperties)
    {
        if (selectProperties == null || selectProperties.Length == 0)
            return "*";

        var props = selectProperties
            .SelectMany(p => EntityMetadataHelper.GetPropertiesFromExpression(p))
            .ToList();

        return string.Join(", ", props.Select(p =>
        {
            var meta = _meta.Properties.Single(x => x.Name == p);
            return _meta.ColumnMappings[meta];
        }));
    }

    private string BuildWhere()
    {
        return string.Join(" AND ",
            _meta.KeyProperties.Select(p =>
                $"{_meta.ColumnMappings[p]} = @{p.Name}"
            ));
    }

    private string BuildInsert()
    {
        var props = _meta.Properties
            .Where(p => !_meta.IdentityProperties.Contains(p))
            .ToList();

        var cols = string.Join(", ", props.Select(p => _meta.ColumnMappings[p]));
        var vals = string.Join(", ", props.Select(p => "@" + p.Name));

        return $"INSERT INTO {_meta.TableName} ({cols}) VALUES ({vals})";
    }

    private string BuildUpdate(params Expression<Func<T, object>>[] includeProperties)
    {
        var props = includeProperties
            .SelectMany(p => EntityMetadataHelper.GetPropertiesFromExpression(p))
            .ToList();

        var set = string.Join(", ", props.Select(p =>
        {
            var meta = _meta.Properties.Single(x => x.Name == p);
            return $"{_meta.ColumnMappings[meta]} = @{p}";
        }));

        return $"UPDATE {_meta.TableName} SET {set} WHERE {BuildWhere()}";
    }
}