using System.Net.Http.Json;
using EcoTracker.ViewModels;

namespace EcoTracker.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/Auth/login";
        var loginModel = new LoginViewModel
        {
            Username = "admin",
            Password = "Admin@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(request, loginModel);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
