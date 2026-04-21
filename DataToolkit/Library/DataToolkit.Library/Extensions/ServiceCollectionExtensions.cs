using DataToolkit.Library.Common;
using DataToolkit.Library.Engine.Resilience;
using DataToolkit.Library.UnitOfWorkLayer;
using Microsoft.Extensions.DependencyInjection;

namespace DataToolkit.Library.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataToolkit(
        this IServiceCollection services,
        Action<DataToolkitOptions>? configure = null)
    {
        var options = new DataToolkitOptions();

        configure?.Invoke(options);

        services.AddSingleton(options);

        // 🔹 Resiliencia
        services.AddSingleton<RetryPolicy>();
        services.AddSingleton<IExecutionPolicy, RetryEngine>();

        // 🔹 Core
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}