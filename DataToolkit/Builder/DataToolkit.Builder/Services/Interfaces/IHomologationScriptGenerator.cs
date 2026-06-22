
using DataToolkit.Builder.Models.Responses;

namespace DataToolkit.Builder.Services.Interfaces;

public interface IHomologationScriptGenerator
{
    Task<string> GenerateHomologationScriptAsync(
        ReferenceDataMappingResult mapping,
        string outputPath);
}