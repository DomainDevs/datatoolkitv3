using DataToolkit.Library.Extensions;
using DataToolkit.Provider.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;

namespace DataToolkit.Sample.Basic.Infrastructure;

public static class DataToolkitConfiguration
{
    public static IServiceCollection AddDataToolkitSample(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDataToolkit(configuration)
            .AddDataToolkitSqlServer();

        return services;
    }
}