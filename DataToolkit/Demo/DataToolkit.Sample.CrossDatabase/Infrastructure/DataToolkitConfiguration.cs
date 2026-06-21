using DataToolkit.Library.Extensions;
using DataToolkit.Provider.Sqlite.Extensions;
using DataToolkit.Provider.SqlServer.Extensions;

namespace DataToolkit.Sample.CrossDatabase.Infrastructure;

public static class DataToolkitConfiguration
{
    public static IServiceCollection AddSqlServerSample(
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

    public static IServiceCollection AddSqliteSample(
        this IServiceCollection services)
    {
        services.AddDataToolkit(options =>
        {
            options.DefaultConnectionAlias = "Sqlite";

            options.Logging.Enabled = true;
            options.Telemetry.Enabled = true;
        });

        services.AddDataToolkitSqlite();

        return services;
    }
}