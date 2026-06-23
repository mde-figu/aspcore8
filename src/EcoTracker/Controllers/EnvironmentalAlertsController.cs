using EcoTracker.Services;
using EcoTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnvironmentalAlertsController : ControllerBase
{
    private readonly IEnvironmentalAlertService _service;

    public EnvironmentalAlertsController(IEnvironmentalAlertService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista alertas ambientais com paginação e filtros.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EnvironmentalAlertResponseViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? company = null,
        [FromQuery] string? severity = null,
        [FromQuery] bool? resolved = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _service.GetAllAsync(page, pageSize, company, severity, resolved);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um alerta ambiental por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnvironmentalAlertResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound(new { Message = $"Alerta com ID {id} não encontrado." });
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo alerta ambiental.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EnvironmentalAlertResponseViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] EnvironmentalAlertCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Resolve um alerta ambiental.
    /// </summary>
    [HttpPatch("{id:int}/resolve")]
    [Authorize]
    [ProducesResponseType(typeof(EnvironmentalAlertResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Resolve(int id, [FromBody] EnvironmentalAlertResolveViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.ResolveAsync(id, model);
        if (result is null) return NotFound(new { Message = $"Alerta com ID {id} não encontrado." });
        return Ok(result);
    }

    /// <summary>
    /// Remove um alerta ambiental.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { Message = $"Alerta com ID {id} não encontrado." });
        return NoContent();
    }
}
