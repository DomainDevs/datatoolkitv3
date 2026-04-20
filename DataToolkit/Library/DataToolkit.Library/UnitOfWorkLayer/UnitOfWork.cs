using DataToolkit.Library.Common;
using DataToolkit.Library.Connections.Abstractions;
using DataToolkit.Library.Engine.Abstractions;
using DataToolkit.Library.Engine.Core;

using Microsoft.Extensions.Options;
using Serilog;
using System.Data;


namespace DataToolkit.Library.UnitOfWorkLayer;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private readonly ILogger _logger = Log.ForContext<UnitOfWork>();
    private readonly bool _disposeConnection;
    private bool _disposed;

    private readonly Dictionary<Type, object> _repositories = new();

    /// <summary>
    /// Only infrastructure-level access. Do not use in application layer.
    /// </summary>
    internal IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public ISqlExecutor Sql { get; }

    public bool HasActiveTransaction => _transaction != null;

    public UnitOfWork(
        IDbConnectionFactory factory,
        string dbAlias = "SqlServer",
        bool disposeConnection = false)
    {
        _connection = factory.CreateConnection(dbAlias)
            ?? throw new InvalidOperationException("Connection factory returned null.");

        _disposeConnection = disposeConnection;

        EnsureOpen();

        Sql = new SqlExecutor(_connection, () => _transaction);
    }

    // ---------------- TRANSACTION SAFETY ----------------

    public void BeginTransaction()
    {
        ThrowIfDisposed();
        EnsureOpen();

        if (_transaction != null)
            throw new InvalidOperationException("Transaction already active.");

        _transaction = _connection.BeginTransaction();
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
        catch (Exception ex)
        {
            _logger.Error(ex, "Commit failed");
            throw;
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
        catch (Exception ex)
        {
            _logger.Error(ex, "Rollback failed");
            throw;
        }
        finally
        {
            ClearTransaction();
        }
    }

    // ---------------- REPOSITORY ----------------
    /*
    public IGenericRepository<T> Repository<T>() where T : class
    {
        ThrowIfDisposed();

        // Guardrail opcional: detecta uso sin transacción
        if (_transaction == null)
            _logger.Warning("Repository used without active transaction: {Entity}", typeof(T).Name);

        if (_repositories.TryGetValue(typeof(T), out var repo))
            return (IGenericRepository<T>)repo;

        EnsureOpen();

        var instance = new GenericRepository<T>(_connection, () => _transaction);
        _repositories[typeof(T)] = instance;
        return instance;
    }*/

    // ---------------- INTERNAL CONTROL ----------------

    private void ClearTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
        _repositories.Clear();
    }

    private void EnsureOpen()
    {
        if (_connection.State == ConnectionState.Broken)
            throw new InvalidOperationException("Connection is broken.");

        if (_disposeConnection && _connection.State != ConnectionState.Closed)
            _connection.Open();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork));
    }

    // ---------------- DISPOSE (SAFE + CONTROLLED) ----------------

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _transaction?.Dispose();

            if (_disposeConnection && _connection.State != ConnectionState.Closed)
                _connection.Dispose();

            _repositories.Clear();
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}