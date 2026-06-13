using DataToolkit.Library.Connections;
using DataToolkit.Provider.SqlServer.Connections.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Provider.SqlServer.Extensions;

public static class SqlServerServiceCollectionExtensions
{
    public static IServiceCollection AddDataToolkitSqlServer(
        this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory,
            SqlServerConnectionFactory>();

        return services;
    }
}