using DataToolkit.Library.Connections;
using DataToolkit.Library.Engine.Abstractions;
using DataToolkit.Library.Engine.Core;
using DataToolkit.Library.Repositories;
using Serilog;
using System.Data;

namespace DataToolkit.Library.UnitOfWorkLayer;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnectionFactory _factory;
    private readonly string _dbAlias;

    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    private readonly ILogger _logger = Log.ForContext<UnitOfWork>();
    private readonly Dictionary<Type, object> _repositories = new();

    private bool _disposed;

    public IDbTransaction? Transaction => _transaction;
    public bool HasActiveTransaction => _transaction != null;

    public ISqlExecutor Sql { get; }

    public UnitOfWork(
        IDbConnectionFactory factory,
        string dbAlias = "SqlServer")
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _dbAlias = dbAlias;

        // ❗ Connection is created but NOT opened here (lazy ownership rule)
        Sql = new SqlExecutor(
            GetConnection,
            GetTransaction);
    }

    // =========================================================
    // SINGLE OWNERSHIP: CONNECTION CONTROLLED ONLY HERE
    // =========================================================

    private IDbConnection GetConnection()
    {
        ThrowIfDisposed();

        if (_connection == null)
        {
            _connection = _factory.CreateConnection(_dbAlias)
                ?? throw new InvalidOperationException("Connection factory returned null.");
        }

        EnsureOpen();
        return _connection;
    }

    private IDbTransaction? GetTransaction() => _transaction;

    private void EnsureOpen()
    {
        if (_connection == null) return;

        if (_connection.State == ConnectionState.Broken)
            throw new InvalidOperationException("Connection is broken.");

        if (_connection.State == ConnectionState.Closed)
            _connection.Open();
    }

    // =========================================================
    // TRANSACTIONS
    // =========================================================

    public void BeginTransaction()
    {
        ThrowIfDisposed();

        var conn = GetConnection();

        if (_transaction != null)
            throw new InvalidOperationException("Transaction already active.");

        _transaction = conn.BeginTransaction();
        _repositories.Clear();
    }

    public void Commit()
    {
        ThrowIfDisposed();

        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            _transaction.Commit();
        }
        finally
        {
            ClearTransaction();
        }
    }

    public void Rollback()
    {
        ThrowIfDisposed();

        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to rollback.");

        try
        {
            _transaction.Rollback();
        }
        finally
        {
            ClearTransaction();
        }
    }

    private void ClearTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
        _repositories.Clear();
    }

    // =========================================================
    // REPOSITORIES
    // =========================================================

    public IGenericRepository<T> Repository<T>() where T : class
    {
        ThrowIfDisposed();

        if (_repositories.TryGetValue(typeof(T), out var repo))
            return (IGenericRepository<T>)repo;

        var instance = new GenericRepository<T>(Sql);
        //_repositories[typeof(T)] = instance;
        _repositories.TryAdd(typeof(T), instance);

        return instance;
    }

    // =========================================================
    // SAFETY
    // =========================================================

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork));
    }

    // =========================================================
    // DISPOSE (OWNERSHIP RULE)
    // =========================================================

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _transaction?.Dispose();
            
            _connection?.Dispose();
            _connection = null;

            _repositories.Clear();
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}