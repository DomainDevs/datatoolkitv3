using DataToolkit.Builder.Models.Requests;

namespace DataToolkit.Builder.Services.Homologation.Interfaces;

public interface IHomologationGenerationService
{
    Task<HomologationGenerationResult> GenerateAsync(
        ReferenceDataMappingRequest request);
}