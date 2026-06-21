using DataToolkit.Library.Connections;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataToolkit.Provider.Sqlite.Connections.Providers;

public sealed class SqliteConnectionFactory
    : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqliteConnectionFactory(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection(
        string dbAlias)
    {
        var connectionString =
            _configuration.GetConnectionString(dbAlias);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No se encontró la cadena de conexión para '{dbAlias}'.");
        }

        return new SqliteConnection(connectionString);
    }
}