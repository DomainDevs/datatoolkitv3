namespace DataToolkit.Library.Engine.Resilience;

public class RetryPolicy
{
    public int MaxRetries { get; }
    public int BaseDelayMs { get; }

    public RetryPolicy(int maxRetries = 3, int baseDelayMs = 200)
    {
        MaxRetries = maxRetries;
        BaseDelayMs = baseDelayMs;
    }

    public bool ShouldRetry(Exception ex, int attempt)
    {
        if (attempt >= MaxRetries)
            return false;

        var type = SqlErrorClassifier.Classify(ex);

        return type == SqlErrorType.Transient;
    }

    public int GetDelay(int attempt)
    {
        return BaseDelayMs * (int)Math.Pow(2, attempt); // exponential backoff
    }
}