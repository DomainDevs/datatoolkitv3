using DataToolkit.Library.Connections;
using DataToolkit.Library.Connections.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Library.Extensions;

public static class SybaseServiceCollectionExtensions
{
    public static IServiceCollection AddDataToolkitSybase(
        this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory,
            SybaseConnectionFactory>();

        return services;
    }
}