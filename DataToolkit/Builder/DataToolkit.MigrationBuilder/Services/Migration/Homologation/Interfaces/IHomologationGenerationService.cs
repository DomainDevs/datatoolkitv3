using DataToolkit.MigrationBuilder.Models.Requests;

namespace DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;

public interface IHomologationGenerationService
{
    Task<HomologationGenerationResult> GenerateAsync(
        ReferenceDataMappingRequest request);
}