namespace DataToolkit.Library.Engine.Resilience;

public class RetryEngine : IExecutionPolicy
{
    private readonly RetryPolicy _policy;

    public RetryEngine(RetryPolicy policy)
    {
        _policy = policy;
    }

    public T Execute<T>(Func<T> action)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                attempt++;

                if (!_policy.ShouldRetry(ex, attempt))
                    throw;

                Thread.Sleep(_policy.GetDelay(attempt));
            }
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                attempt++;

                if (!_policy.ShouldRetry(ex, attempt))
                    throw;

                await Task.Delay(_policy.GetDelay(attempt));
            }
        }
    }
}