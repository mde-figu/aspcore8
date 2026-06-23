namespace EcoTracker.Tests;

public class EnvironmentalAlertsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EnvironmentalAlertsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalAlerts";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetById_WithSeedData_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalAlerts/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithSeverityFilter_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalAlerts?severity=Critical";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_UnresolvedOnly_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/EnvironmentalAlerts?resolved=false";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
