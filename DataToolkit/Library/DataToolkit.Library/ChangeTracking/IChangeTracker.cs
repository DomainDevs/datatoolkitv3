namespace DataToolkit.Library.ChangeTracking;

public interface IChangeTracker
{
    EntityEntry<T> Track<T>(T entity) where T : class;
    EntityEntry<T>? Entry<T>(T entity) where T : class;
    IReadOnlyCollection<object> TrackedEntities { get; }
}