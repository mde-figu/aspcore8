using EcoTracker.Services;
using EcoTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarbonEmissionsController : ControllerBase
{
    private readonly ICarbonEmissionService _service;

    public CarbonEmissionsController(ICarbonEmissionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista emissões de carbono com paginação e filtros.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CarbonEmissionResponseViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? company = null,
        [FromQuery] string? sector = null,
        [FromQuery] int? year = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _service.GetAllAsync(page, pageSize, company, sector, year);
        return Ok(result);
    }

    /// <summary>
    /// Obtém uma emissão de carbono por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CarbonEmissionResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound(new { Message = $"Emissão com ID {id} não encontrada." });
        return Ok(result);
    }

    /// <summary>
    /// Registra uma nova emissão de carbono.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CarbonEmissionResponseViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CarbonEmissionCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza uma emissão de carbono existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CarbonEmissionResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] CarbonEmissionUpdateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, model);
        if (result is null) return NotFound(new { Message = $"Emissão com ID {id} não encontrada." });
        return Ok(result);
    }

    /// <summary>
    /// Remove uma emissão de carbono.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { Message = $"Emissão com ID {id} não encontrada." });
        return NoContent();
    }
}
