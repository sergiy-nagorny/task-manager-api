using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskManager.Api.Tests;

public class DiagnosticsEndpointsIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DiagnosticsEndpointsIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Ping_Returns200()
    {
        var response = await _client.GetAsync("/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ping_ReturnsStatusOkBody()
    {
        var response = await _client.GetAsync("/ping");
        var body = await response.Content.ReadFromJsonAsync<DiagnosticsResponse>();

        Assert.Equal("ok", body!.Status);
    }

    [Fact]
    public async Task Ready_Returns200()
    {
        var response = await _client.GetAsync("/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_ReturnsChecksArray()
    {
        var response = await _client.GetAsync("/ready");
        var body = await response.Content.ReadFromJsonAsync<ReadinessResponse>();

        Assert.Equal("ok", body!.Status);
        var check = Assert.Single(body.Checks);
        Assert.Equal("task-store", check.Name);
        Assert.Equal("ok", check.Status);
    }
}
