using DataToolkit.Library.Extensions;
using DataToolkit.Provider.SqlServer.Extensions;

namespace DataToolkit.Sample.Basic.Infrastructure;

public static class DataToolkitConfiguration
{
    public static IServiceCollection AddDataToolkitSample(
        this IServiceCollection services)
    {
        services.AddDataToolkit(options =>
        {
            options.DefaultConnectionAlias = "SqlServer";

            options.Logging.Enabled = true;
            options.Telemetry.Enabled = true;
        });

        services.AddDataToolkitSqlServer();

        return services;
    }
}