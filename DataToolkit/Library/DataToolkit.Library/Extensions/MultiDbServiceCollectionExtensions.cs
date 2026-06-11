using DataToolkit.Library.Connections;
using DataToolkit.Library.Infrastructure.ConnectionRouting;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Library.Extensions;

public static class MultiDbServiceCollectionExtensions
{
    public static IServiceCollection AddDataToolkitMultiDb(
        this IServiceCollection services,
        Dictionary<string,
            (string connectionString,
             DatabaseProvider provider)> config)
    {
        services.AddSingleton<IConnectionResolver>(
            new InMemoryConnectionResolver(config));

        services.AddScoped<IDbConnectionFactory,
            MultiDbConnectionFactory>();

        return services;
    }
}