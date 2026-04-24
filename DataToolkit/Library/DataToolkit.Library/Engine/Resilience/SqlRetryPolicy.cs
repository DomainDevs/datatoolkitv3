using Microsoft.Data.SqlClient;

namespace DataToolkit.Library.Engine.Resilience;

public sealed class SqlRetryPolicy : IRetryPolicy
{
    private readonly int _maxRetries;
    private readonly int _baseDelayMs;

    public SqlRetryPolicy(int maxRetries = 3, int baseDelayMs = 200)
    {
        _maxRetries = maxRetries;
        _baseDelayMs = baseDelayMs;
    }

    public bool ShouldRetry(Exception ex, int attempt)
    {
        if (attempt >= _maxRetries)
            return false;

        if (ex is not SqlException sqlEx)
            return false;

        return sqlEx.Number switch
        {
            1205 => true,  // deadlock
            40197 => true, // azure transient
            40501 => true,
            40613 => true,
            10928 => true,
            10929 => true,
            _ => false
        };
    }

    public TimeSpan GetDelay(int attempt)
    {
        var ms = _baseDelayMs * Math.Pow(2, attempt);
        return TimeSpan.FromMilliseconds(ms);
    }
}