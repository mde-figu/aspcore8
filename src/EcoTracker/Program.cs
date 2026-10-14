using System.Text;
using EcoTracker.Data;
using EcoTracker.Middleware;
using EcoTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<EcoTrackerDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<EcoTrackerDbContext>(options =>
        options.UseInMemoryDatabase("EcoTrackerDb"));
}

// --- Services (DI) ---
builder.Services.AddScoped<ICarbonEmissionService, CarbonEmissionService>();
builder.Services.AddScoped<IEnvironmentalLicenseService, EnvironmentalLicenseService>();
builder.Services.AddScoped<IComplianceAuditService, ComplianceAuditService>();
builder.Services.AddScoped<IEnvironmentalAlertService, EnvironmentalAlertService>();

// --- Authentication (JWT) ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? "EcoTracker-SecretKey-2024-ESG-Compliance-Min32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EcoTracker";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EcoTrackerUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// --- Health checks ---
builder.Services.AddHealthChecks();

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoTracker API",
        Version = "v1",
        Description = "API RESTful para Governança e Compliance Ambiental (ESG) — Tema 4. " +
                      "Monitoramento de emissões de carbono, licenças ambientais, auditorias de compliance e alertas ambientais."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT. Exemplo: eyJhbGciOi..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Middleware ---
app.UseMiddleware<GlobalExceptionMiddleware>();

// --- Swagger (dev, staging e demais ambientes com EnableSwagger=true) ---
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoTracker API v1"));
}

if (!builder.Configuration.GetValue<bool>("DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

// Cria o schema (SQL Server) ou popula o banco InMemory, aguardando o banco subir
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EcoTrackerDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var attempts = builder.Configuration.GetValue("Database:StartupRetries", 10);

    for (var attempt = 1; ; attempt++)
    {
        try
        {
            context.Database.EnsureCreated();
            break;
        }
        catch (Exception ex) when (attempt < attempts)
        {
            logger.LogWarning(ex, "Banco indisponível (tentativa {Attempt}/{Attempts}); nova tentativa em 5s", attempt, attempts);
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

app.Run();

// Exposição parcial da classe Program para permitir testes de integração com WebApplicationFactory
public partial class Program { }
