using AdoNetCore.AseClient;
using Microsoft.Data.SqlClient;
using System.Data;
using DataToolkit.Library.Connections;
using DataToolkit.Library.Infrastructure.ConnectionRouting;

namespace DataToolkit.Library.Infrastructure.ConnectionRouting;

public class MultiDbConnectionFactory : IDbConnectionFactory
{
    private readonly IConnectionResolver _resolver;

    public MultiDbConnectionFactory(IConnectionResolver resolver)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    public IDbConnection CreateConnection(string alias = "MainSql")
    {
        var (connectionString, provider) = _resolver.Resolve(alias);

        return provider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(connectionString),
            DatabaseProvider.Sybase => new AseConnection(connectionString),
            _ => throw new NotSupportedException($"Proveedor no soportado: {provider}")
        };
    }
}