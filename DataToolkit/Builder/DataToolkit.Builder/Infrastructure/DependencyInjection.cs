using DataToolkit.Builder.Configuration;
using DataToolkit.Builder.Services;
using DataToolkit.Library.Extensions;
using DataToolkit.Provider.SqlServer.Extensions;

namespace DataToolkit.Builder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuilderServices(
        this IServiceCollection services)
    {

        services.AddScoped<MetadataService>();
        services.AddScoped<MigrationMetadataService>();
        services.AddScoped<MigrationWorkFileService>();
        services.AddScoped<MigrationSqlGeneratorService>();

        return services;
    }
}
