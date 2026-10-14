namespace EcoTracker.Tests;

public class CarbonEmissionsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CarbonEmissionsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/CarbonEmissions";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetById_WithSeedData_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/CarbonEmissions/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithPagination_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/CarbonEmissions?page=1&pageSize=5";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithFilters_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/CarbonEmissions?sector=Energia&year=2024";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
