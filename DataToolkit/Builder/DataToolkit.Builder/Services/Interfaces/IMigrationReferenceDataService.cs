
using DataToolkit.Builder.Models.Responses;

namespace DataToolkit.Builder.Services.Interfaces;

public interface IMigrationReferenceDataService
{
    Task<string> GenerateHomologationScriptAsync(
        ReferenceDataMappingResult mapping,
        string outputPath);
}