/*
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
        // OPTIONS
        services.Configure<DataToolkitOptions>(configure ?? (_ => { }));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<DataToolkitOptions>>().Value);

        // CONNECTIONS
        services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();

        // RETRY POLICY (CORE)
        services.AddSingleton<IExecutionPolicy, RetryEngine>();

        // SQL EXECUTOR BASE
        services.AddScoped<SqlExecutor>();

        // RESILIENT DECORATION (MANUAL, SIN SCRUTOR)
        services.AddScoped<ISqlExecutor>(sp =>
        {
            var factory = sp.GetRequiredService<IDbConnectionFactory>();
            var policy = sp.GetRequiredService<IExecutionPolicy>();

            var connection = factory.CreateConnection("SqlServer");

            var inner = new SqlExecutor(connection, () => null);

            return new ResilientSqlExecutor(inner, policy);
        });

        // UNIT OF WORK (OWNER DE CONEXIÓN)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
*/