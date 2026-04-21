using DataToolkit.Library.Common;
using DataToolkit.Library.Connections;
using DataToolkit.Library.Connections.Providers;
using DataToolkit.Library.Engine.Abstractions;
using DataToolkit.Library.Engine.Core;
using DataToolkit.Library.Engine.Resilience;
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
        services.Configure<DataToolkitOptions>(configure ?? (_ => { }));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<DataToolkitOptions>>().Value);

        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();

        services.AddSingleton<IExecutionPolicy, RetryEngine>();

        services.AddScoped<ISqlExecutor, SqlExecutor>();

        services.Decorate<ISqlExecutor, ResilientSqlExecutor>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}