using DataToolkit.MigrationBuilder.Configuration;
using DataToolkit.MigrationBuilder.Models.Requests;
using DataToolkit.MigrationBuilder.Services.Migration.Homologation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public sealed class HomologationController
    : ControllerBase
{
    private readonly IHomologationGenerationService _service;

    public HomologationController(
        IHomologationGenerationService service
        )
    {
        _service = service;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<HomologationGenerationResult>>
        Generate(
            [FromBody] ReferenceDataMappingRequest request)
    {
        var result =
            await _service.GenerateAsync(
                request);

        return Ok(result);
    }
}