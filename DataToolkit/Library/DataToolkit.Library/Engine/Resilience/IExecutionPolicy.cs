namespace DataToolkit.Library.Engine.Resilience;

internal interface IExecutionPolicy
{
    T Execute<T>(Func<T> action);
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
}