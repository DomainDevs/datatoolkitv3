using DataToolkit.Library.UnitOfWorkLayer;

namespace DataToolkit.Library.Context;

public sealed class DataContext : IDataContext
{
    private readonly IUnitOfWork _uow;

    public DataContext(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ---------------- QUERY ----------------

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        => _uow.Sql.FromSqlAsync<T>(sql, param);

    public Task<IEnumerable<T>> Query<T>(string sql, object? param = null)
        => _uow.Sql.FromSqlAsync<T>(sql, param);

    // ---------------- EXECUTE ----------------

    public Task<int> ExecuteAsync(string sql, object? param = null)
        => _uow.Sql.ExecuteAsync(sql, param);

    // ---------------- TRANSACTION (solo passthrough) ----------------

    public void BeginTransaction() => _uow.BeginTransaction();

    public void Commit() => _uow.Commit();

    public void Rollback() => _uow.Rollback();
}