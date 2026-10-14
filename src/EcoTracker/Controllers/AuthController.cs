using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EcoTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Autentica o usuário e retorna um token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Usuários de demonstração para fins acadêmicos
        var (isValid, role) = ValidateCredentials(model.Username, model.Password);
        if (!isValid)
            return Unauthorized(new { Message = "Credenciais inválidas." });

        var token = GenerateJwtToken(model.Username, role);

        return Ok(new TokenResponseViewModel
        {
            Token = token.Token,
            Expiration = token.Expiration,
            Username = model.Username,
            Role = role
        });
    }

    private static (bool IsValid, string Role) ValidateCredentials(string username, string password)
    {
        // Usuários de demonstração — em produção, usar Identity/banco de dados
        var demoUsers = new Dictionary<string, (string Password, string Role)>
        {
            ["admin"] = ("Admin@123", "Admin"),
            ["auditor"] = ("Auditor@123", "Auditor"),
            ["viewer"] = ("Viewer@123", "Viewer")
        };

        if (demoUsers.TryGetValue(username.ToLowerInvariant(), out var user) && user.Password == password)
            return (true, user.Role);

        return (false, string.Empty);
    }

    private (string Token, DateTime Expiration) GenerateJwtToken(string username, string role)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "EcoTracker-SecretKey-2024-ESG-Compliance-Min32Chars!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var expiration = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "EcoTracker",
            audience: _configuration["Jwt:Audience"] ?? "EcoTrackerUsers",
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
}
