using DataToolkit.MigrationBuilder.Configuration;
using DataToolkit.MigrationBuilder.Services;
using DataToolkit.MigrationBuilder.Services.Migration;
using DataToolkit.MigrationBuilder.Services.Migration.Homologation;
using DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;
using DataToolkit.Library.Extensions;
using DataToolkit.Provider.SqlServer.Extensions;

namespace DataToolkit.MigrationBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuilderServices(
        this IServiceCollection services)
    {

        services.AddScoped<MetadataService>();
        services.AddScoped<MigrationMetadataService>();
        services.AddScoped<MigrationWorkFileService>();
        services.AddScoped<MigrationSqlGeneratorService>();
        services.AddScoped<MigrationDdlGeneratorService>();
        services.AddScoped<MigrationDependencyService>();
        services.AddScoped<IHomologationArtifactGenerator, HomologationArtifactGenerator>();
        services.AddScoped<IHomologationGenerationService, HomologationGenerationService>();
        services.AddScoped<IHomologationDiscoveryService, HomologationDiscoveryService>();
        

        return services;
    }
}
