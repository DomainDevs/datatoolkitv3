using System.Data;

namespace DataToolkit.Library.Connections.Abstractions;
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection(string dbAlias);
}
