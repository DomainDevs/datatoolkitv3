using DataToolkit.Library.Common;
using DataToolkit.Library.Connections;
using DataToolkit.Library.Engine.Resilience;

//using DataToolkit.Library.Engine.Resilience;
using DataToolkit.Library.UnitOfWorkLayer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataToolkit.Library.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataToolkit(
        this IServiceCollection services,
        Action<DataToolkitOptions>? configure = null)
    {
        services.Configure(configure ?? (_ => { }));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<DataToolkitOptions>>().Value);

        services.AddSingleton<RetryExecutor>();

        services.AddScoped<IUnitOfWork>(sp =>
        {
            var factory =
                sp.GetRequiredService<IDbConnectionFactory>();

            var options =
                sp.GetRequiredService<DataToolkitOptions>();

            return new UnitOfWork(
                factory,
                options.DefaultConnectionAlias);
        });

        return services;
    }
}