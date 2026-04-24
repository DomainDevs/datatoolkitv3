using DataToolkit.Library.Engine.Abstractions;

namespace DataToolkit.Library.Engine.Decorators;

internal sealed class ResilientSqlExecutor
{
    private readonly ISqlExecutor _inner;
    private readonly RetryExecutor _retry;

    public ResilientSqlExecutor(
        ISqlExecutor inner,
        RetryExecutor retry)
    {
        _inner = inner;
        _retry = retry;
    }

    public Task<IEnumerable<T>> FromSqlAsync<T>(
        string sql,
        object? parameters = null,
        int? commandTimeout = null)
        => _retry.ExecuteAsync(() =>
            _inner.FromSqlAsync<T>(sql, parameters, commandTimeout));

    public Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        int? commandTimeout = null)
        => _retry.ExecuteAsync(() =>
            _inner.ExecuteAsync(sql, parameters, commandTimeout));

    // Puedes ir agregando solo los métodos críticos
}