public sealed class RetryExecutor
{
    private readonly IRetryPolicy _policy;

    private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MinDelay = TimeSpan.Zero;

    public RetryExecutor(IRetryPolicy policy)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    public Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        return ExecuteInternalAsync(action);
    }

    private async Task<T> ExecuteInternalAsync<T>(Func<Task<T>> action)
    {
        var attempt = 0;

        while (true)
        {
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                attempt++;

                if (!_policy.ShouldRetry(ex, attempt))
                    throw;

                var delay = NormalizeDelay(_policy.GetDelay(attempt));

                await Task.Delay(delay).ConfigureAwait(false);
            }
        }
    }

    private static TimeSpan NormalizeDelay(TimeSpan delay)
    {
        if (delay <= MinDelay)
            return MinDelay;

        if (delay >= MaxDelay)
            return MaxDelay;

        return delay;
    }
}