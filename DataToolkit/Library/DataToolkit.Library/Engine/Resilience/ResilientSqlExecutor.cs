using Dapper;
using DataToolkit.Library.Engine.Abstractions;
using DataToolkit.Library.Engine.Mapping;
using System.Data;

namespace DataToolkit.Library.Engine.Resilience;

internal class ResilientSqlExecutor : ISqlExecutor
{
    private readonly ISqlExecutor _inner;
    private readonly IExecutionPolicy _policy;

    public ResilientSqlExecutor(ISqlExecutor inner, IExecutionPolicy policy)
    {
        _inner = inner;
        _policy = policy;
    }

    public int Execute(string sql)
        => _policy.Execute(() => _inner.Execute(sql));

    public int Execute(string sql, object? parameters = null, int? commandTimeout = null)
        => _policy.Execute(() => _inner.Execute(sql, parameters, commandTimeout));

    public int Execute(string sql, object? parameters)
        => _policy.Execute(() => _inner.Execute(sql, parameters));

    public Task<int> ExecuteAsync(string sql)
        => _policy.ExecuteAsync(() => _inner.ExecuteAsync(sql));

    public Task<int> ExecuteAsync(string sql, object? parameters = null, int? commandTimeout = null)
        => _policy.ExecuteAsync(() => _inner.ExecuteAsync(sql, parameters, commandTimeout));

    public Task<int> ExecuteAsync(string sql, object? parameters)
        => _policy.ExecuteAsync(() => _inner.ExecuteAsync(sql, parameters));

    public IEnumerable<T> FromSql<T>(string sql)
        => _policy.Execute(() => _inner.FromSql<T>(sql));

    public IEnumerable<T> FromSql<T>(string sql, object? parameters = null, int? commandTimeout = null)
        => _policy.Execute(() => _inner.FromSql<T>(sql, parameters, commandTimeout));

    public IEnumerable<T> FromSql<T>(string sql, object? parameters)
        => _policy.Execute(() => _inner.FromSql<T>(sql, parameters));

    public Task<IEnumerable<T>> FromSqlAsync<T>(string sql)
        => _policy.ExecuteAsync(() => _inner.FromSqlAsync<T>(sql));

    public Task<IEnumerable<T>> FromSqlAsync<T>(string sql, object? parameters = null, int? commandTimeout = null)
        => _policy.ExecuteAsync(() => _inner.FromSqlAsync<T>(sql, parameters, commandTimeout));

    public Task<IEnumerable<T>> FromSqlAsync<T>(string sql, object? parameters)
        => _policy.ExecuteAsync(() => _inner.FromSqlAsync<T>(sql, parameters));

    public IEnumerable<T> FromSqlInterpolated<T>(FormattableString query)
        => _policy.Execute(() => _inner.FromSqlInterpolated<T>(query));

    public IEnumerable<T> FromSqlInterpolated<T>(FormattableString query, int? commandTimeout = null)
        => _policy.Execute(() => _inner.FromSqlInterpolated<T>(query, commandTimeout));

    public Task<IEnumerable<T>> FromSqlInterpolatedAsync<T>(FormattableString query, int? commandTimeout = null, CancellationToken ct = default)
        => _policy.ExecuteAsync(() => _inner.FromSqlInterpolatedAsync<T>(query, commandTimeout, ct));

    public IEnumerable<T> FromSqlMultiMap<T>(MultiMapRequest request)
        => _policy.Execute(() => _inner.FromSqlMultiMap<T>(request));

    public IEnumerable<T> FromSqlMultiMap<T>(MultiMapRequest request, int? commandTimeout = null)
        => _policy.Execute(() => _inner.FromSqlMultiMap<T>(request, commandTimeout));

    public Task<IEnumerable<T>> FromSqlMultiMapAsync<T>(MultiMapRequest request, int? commandTimeout = null)
        => _policy.ExecuteAsync(() => _inner.FromSqlMultiMapAsync<T>(request, commandTimeout));

    public Task<List<IEnumerable<dynamic>>> QueryMultipleAsync(string sql, object? parameters = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
        => _policy.ExecuteAsync(() => _inner.QueryMultipleAsync(sql, parameters, commandType, commandTimeout));

    public (int RowsAffected, Dictionary<string, object> OutputValues) ExecuteWithOutput(string storedProcedure, Action<DynamicParameters> configureParameters)
        => _policy.Execute(() => _inner.ExecuteWithOutput(storedProcedure, configureParameters));

    public (int RowsAffected, Dictionary<string, object> OutputValues) ExecuteWithOutput(string storedProcedure, Action<DynamicParameters> configureParameters, int? commandTimeout = null)
        => _policy.Execute(() => _inner.ExecuteWithOutput(storedProcedure, configureParameters, commandTimeout));

    public Task<(int RowsAffected, DynamicParameters Output)> ExecuteWithOutputAsync(string storedProcedure, Action<DynamicParameters> configureParameters, int? commandTimeout = null)
        => _policy.ExecuteAsync(() => _inner.ExecuteWithOutputAsync(storedProcedure, configureParameters, commandTimeout));

    public void Dispose()
        => _inner.Dispose();
}