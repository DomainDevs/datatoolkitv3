public interface IRetryPolicy
{
    bool ShouldRetry(Exception ex, int attempt);
    TimeSpan GetDelay(int attempt);
}