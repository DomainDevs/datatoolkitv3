using System.Data;
using DataToolkit.Library.Connections;

namespace DataToolkit.Library.Infrastructure.ConnectionRouting;

public interface IConnectionResolver
{
    (string connectionString, DatabaseProvider provider) Resolve(string alias);
}