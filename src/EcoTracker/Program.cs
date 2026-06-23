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

// --- Swagger (dev) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoTracker API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed InMemory database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EcoTrackerDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

// Exposição parcial da classe Program para permitir testes de integração com WebApplicationFactory
public partial class Program { }
