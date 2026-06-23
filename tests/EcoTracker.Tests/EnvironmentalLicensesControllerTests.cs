namespace EcoTracker.Tests;

public class EnvironmentalLicensesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EnvironmentalLicensesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalLicenses";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetById_WithSeedData_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalLicenses/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetExpiring_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalLicenses/expiring?days=365";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithPagination_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalLicenses?page=1&pageSize=5";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
