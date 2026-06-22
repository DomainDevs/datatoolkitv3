using DataToolkit.Builder.Models.Requests;

namespace DataToolkit.Builder.Services.Homologation.Interfaces;

using DataToolkit.Builder.Models.Requests;

public interface IHomologationDiscoveryService
{
    Task<HomologationResult> DiscoverAsync(
        ReferenceDataMappingRequest request);
}