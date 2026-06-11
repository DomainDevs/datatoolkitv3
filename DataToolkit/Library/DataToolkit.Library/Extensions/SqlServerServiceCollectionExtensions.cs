using DataToolkit.Library.Connections;
using DataToolkit.Library.Connections.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Library.Extensions;

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