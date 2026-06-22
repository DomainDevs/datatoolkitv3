using DataToolkit.Builder.Models.Responses;

namespace DataToolkit.Builder.Services.Homologation.Interfaces;

public interface IHomologationArtifactGenerator
{
    Task<string> GenerateHomologationScriptAsync(
        HomologationResult mapping,
        string outputPath);
}