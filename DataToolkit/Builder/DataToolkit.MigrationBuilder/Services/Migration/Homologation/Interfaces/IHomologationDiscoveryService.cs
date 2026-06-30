using DataToolkit.MigrationBuilder.Models.Requests;

namespace DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;

using DataToolkit.MigrationBuilder.Models.Requests;

public interface IHomologationDiscoveryService
{
    Task<HomologationResult> DiscoverAsync(
        ReferenceDataMappingRequest request);
}