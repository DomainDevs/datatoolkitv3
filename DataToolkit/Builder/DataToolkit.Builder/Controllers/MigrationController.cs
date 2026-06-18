using DataToolkit.Builder.Models;
using DataToolkit.Builder.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataToolkit.Builder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly MetadataService _metadataService;
    private readonly MigrationMetadataService _migrationMetadataService;
    private readonly MigrationWorkFileService _workFileService;
    private readonly MigrationSqlGeneratorService _sqlGeneratorService;

    public MigrationController(
    MetadataService metadataService,
    MigrationMetadataService migrationMetadataService,
    MigrationWorkFileService workFileService,
    MigrationSqlGeneratorService sqlGeneratorService)
    {
        _metadataService = metadataService;
        _migrationMetadataService = migrationMetadataService;
        _workFileService = workFileService;
        _sqlGeneratorService = sqlGeneratorService;
    }

    /// <summary>
    /// Compara metadata entre origen y destino.
    /// </summary>
    [HttpPost("compare")]
    public async Task<IActionResult> Compare(
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

        var differences =
            _migrationMetadataService.CompareMetadata(
                sourceMetadata,
                targetMetadata);

        return Ok(differences);
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

        await _migrationMetadataService.GenerateWorkFilesAsync(
            sourceMetadata,
            targetMetadata,
            outputPath);

        return Ok(new
        {
            OutputPath = outputPath,
            FilesGenerated = true
        });
    }

    /// <summary>
    /// Genera WorkFiles de migración.
    /// </summary>
    [HttpPost("workfiles")]
    public async Task<IActionResult> GenerateWorkFiles(
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
                "WF_OUTPUT");

        await _workFileService.GenerateMigrationWorkFilesAsync(
            sourceMetadata,
            targetMetadata,
            outputPath);

        return Ok(new
        {
            OutputPath = outputPath,
            WorkFilesGenerated = true
        });
    }

    /// <summary>
    /// Genera scripts SQL a partir de WorkFiles.
    /// </summary>
    [HttpPost("generate-sql")]
    public async Task<IActionResult> GenerateSql(
        [FromBody] GenerateSqlRequest request)
    {
        await _sqlGeneratorService.GenerateSqlScriptsAsync(request);

        return Ok(new
        {
            ScriptsGenerated = true
        });
    }

}