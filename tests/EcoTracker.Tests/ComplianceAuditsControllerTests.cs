namespace EcoTracker.Tests;

public class ComplianceAuditsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ComplianceAuditsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/ComplianceAudits";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetById_WithSeedData_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/ComplianceAudits/1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithNormFilter_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/ComplianceAudits?norm=ISO";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_WithPagination_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/ComplianceAudits?page=1&pageSize=5";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
