using DataToolkit.Builder.Configuration;
using DataToolkit.Builder.Models.Requests;
using DataToolkit.Builder.Services.Homologation.Interfaces;
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