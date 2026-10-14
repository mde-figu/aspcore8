using EcoTracker.Services;
using EcoTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnvironmentalLicensesController : ControllerBase
{
    private readonly IEnvironmentalLicenseService _service;

    public EnvironmentalLicensesController(IEnvironmentalLicenseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista licenças ambientais com paginação e filtros.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EnvironmentalLicenseResponseViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? company = null,
        [FromQuery] string? status = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _service.GetAllAsync(page, pageSize, company, status);
        return Ok(result);
    }

    /// <summary>
    /// Obtém uma licença ambiental por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnvironmentalLicenseResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result is null) return NotFound(new { Message = $"Licença com ID {id} não encontrada." });
        return Ok(result);
    }

    /// <summary>
    /// Lista licenças próximas do vencimento.
    /// </summary>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(IReadOnlyList<EnvironmentalLicenseResponseViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpiring([FromQuery] int days = 90)
    {
        if (days < 1) days = 90;
        var result = await _service.GetExpiringAsync(days);
        return Ok(result);
    }

    /// <summary>
    /// Registra uma nova licença ambiental.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EnvironmentalLicenseResponseViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] EnvironmentalLicenseCreateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza uma licença ambiental existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(EnvironmentalLicenseResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] EnvironmentalLicenseUpdateViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, model);
        if (result is null) return NotFound(new { Message = $"Licença com ID {id} não encontrada." });
        return Ok(result);
    }

    /// <summary>
    /// Remove uma licença ambiental.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(new { Message = $"Licença com ID {id} não encontrada." });
        return NoContent();
    }
}
