using DataToolkit.Builder.Configuration;
using DataToolkit.Builder.Models.Requests;
using DataToolkit.Builder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataToolkit.Builder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly MetadataService _metadataService;
    private readonly MigrationOptions _migrationOptions;

    public MigrationController(
    IOptions<MigrationOptions> options,
    MetadataService metadataService)
    {
        _metadataService = metadataService;
        _migrationOptions = options.Value;
    }

    /// <summary>
    /// Genera WorkFiles.
    /// </summary>
    [HttpPost("metadata-report")]
    public async Task<IActionResult> GenerateMetadataReport(
        [FromBody] CompareRequest request)
    {
        if (request == null)
            return BadRequest("Request inválido.");

        var sourceMetadata =
            await _metadataService.ExtractMetadataAsync(
                request.SourceConnectionString,
                request.Schema,
                request.Tables);

        var targetMetadata =
            await _metadataService.ExtractMetadataAsync(
                request.TargetConnectionString,
                request.Schema,
                request.Tables);

        var outputPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "METADATA_OUTPUT");

        return Ok(new
        {
            FilesGenerated = true
        });
    }
}