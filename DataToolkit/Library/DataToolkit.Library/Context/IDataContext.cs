
namespace DataToolkit.Library.Context
{
    public interface IDataContext
    {
        void BeginTransaction();
        void Commit();
        Task<int> ExecuteAsync(string sql, object? param = null);
        Task<IEnumerable<T>> Query<T>(string sql, object? param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        void Rollback();
    }
}