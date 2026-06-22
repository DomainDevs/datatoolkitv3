using DataToolkit.Builder.Configuration;
using DataToolkit.Builder.Models.Requests;
using DataToolkit.Builder.Services.Homologation.Interfaces;
using Microsoft.Extensions.Options;

public sealed class HomologationGenerationService
    : IHomologationGenerationService
{
    private readonly IHomologationDiscoveryService _discovery;

    private readonly IHomologationArtifactGenerator _generator;

    private readonly MigrationOptions _options;

    public HomologationGenerationService(
        IHomologationDiscoveryService discovery,
        IHomologationArtifactGenerator generator,
        IOptions<MigrationOptions> options)
    {
        _discovery = discovery;
        _generator = generator;
        _options = options.Value;
    }

    public async Task<HomologationGenerationResult> GenerateAsync(
        ReferenceDataMappingRequest request)
    {
        var result =
            await _discovery.DiscoverAsync(
                request);

        var file =
            await _generator.GenerateHomologationScriptAsync(
                result,
                _options.SqlOutputPath + "/ Homologation");

        return new HomologationGenerationResult
        {
            ScriptPath = file,
            Result = result
        };
    }
}