using DataToolkit.Library.Engine.Abstractions;
using System.Data;

namespace DataToolkit.Library.UnitOfWorkLayer
{
    public interface IUnitOfWork
    {
        bool HasActiveTransaction { get; }
        ISqlExecutor Sql { get; }
        IDbTransaction? Transaction { get; }

        void BeginTransaction();
        void Commit();
        void Dispose();
        //IGenericRepository<T> Repository<T>() where T : class;
        void Rollback();
    }
}