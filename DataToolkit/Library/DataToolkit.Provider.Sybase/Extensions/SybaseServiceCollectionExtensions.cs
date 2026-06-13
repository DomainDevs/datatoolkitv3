using DataToolkit.Library.Connections;
using DataToolkit.Provider.Sybase.Connections.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Provider.Sybase.Extensions;

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
