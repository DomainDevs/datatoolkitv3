
namespace DataToolkit.Library.Context
{
    public interface IDataContext
    {
        void BeginTransaction();
        void Commit();
        void Dispose();
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
        void Rollback();
    }
}