using DataToolkit.MigrationBuilder.Models.Responses;

namespace DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;

public interface IHomologationArtifactGenerator
{
    Task<string> GenerateHomologationScriptAsync(
        HomologationResult mapping,
        string outputPath);
}