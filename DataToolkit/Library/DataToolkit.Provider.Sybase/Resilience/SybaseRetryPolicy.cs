using DataToolkit.Library.Engine.Resilience;

namespace DataToolkit.Provider.Sybase.Resilience;

public sealed class SybaseRetryPolicy : IRetryPolicy
{
    private readonly int _maxRetries;
    private readonly int _baseDelayMs;

    public SybaseRetryPolicy(
        int maxRetries = 3,
        int baseDelayMs = 200)
    {
        _maxRetries = maxRetries;
        _baseDelayMs = baseDelayMs;
    }

    public bool ShouldRetry(Exception ex, int attempt)
    {
        if (attempt >= _maxRetries)
            return false;

        // TODO:
        var message = ex.Message.ToLowerInvariant();

        return
            message.Contains("deadlock") ||
            message.Contains("timeout") ||
            message.Contains("socket") ||
            message.Contains("network") ||
            message.Contains("connection terminated");
    }

    public TimeSpan GetDelay(int attempt)
    {
        var ms = _baseDelayMs * Math.Pow(2, attempt);

        return TimeSpan.FromMilliseconds(ms);
    }
}