using DataToolkit.Library.Common;

namespace DataToolkit.Library.Engine.Resilience;

public class RetryPolicy
{
    private readonly RetryOptions _options;

    public RetryPolicy(DataToolkitOptions toolkitOptions)
    {
        _options = toolkitOptions.Retry;
    }

    public bool ShouldRetry(Exception ex, int attempt)
    {
        if (!_options.Enabled)
            return false;

        if (attempt >= _options.MaxRetries)
            return false;

        var type = SqlErrorClassifier.Classify(ex);

        return type == SqlErrorType.Transient;
    }

    public int GetDelay(int attempt)
    {
        return _options.BaseDelayMs * (int)Math.Pow(2, attempt);
    }
}