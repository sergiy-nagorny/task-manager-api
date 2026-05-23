using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskManager.Api.Tests;

public class TaskEndpointsIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskEndpointsIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private async Task<TaskItem> PostTaskAsync(string title, string? description = null)
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { title, description });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskItem>())!;
    }

    [Fact]
    public async Task GetAll_Empty_Returns200WithEmptyArray()
    {
        var response = await _client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<TaskItem[]>();
        Assert.Empty(tasks!);
    }

    [Fact]
    public async Task GetAll_WithTasks_ReturnsAllTasks()
    {
        await PostTaskAsync("Task One");
        await PostTaskAsync("Task Two");

        var response = await _client.GetAsync("/tasks");
        var tasks = await response.Content.ReadFromJsonAsync<TaskItem[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, tasks!.Length);
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200WithTask()
    {
        var created = await PostTaskAsync("My Task");

        var response = await _client.GetAsync($"/tasks/{created.Id}");
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created.Id, task!.Id);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocationHeader()
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { title = "New Task" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Create_CreatedTaskIsRetrievable()
    {
        var created = await PostTaskAsync("Retrievable Task", "some description");

        var response = await _client.GetAsync($"/tasks/{created.Id}");
        var fetched = await response.Content.ReadFromJsonAsync<TaskItem>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Retrievable Task", fetched!.Title);
        Assert.Equal("some description", fetched.Description);
    }

    [Fact]
    public async Task Update_ExistingId_Returns200WithUpdatedTask()
    {
        var created = await PostTaskAsync("Original Title");

        var response = await _client.PutAsJsonAsync($"/tasks/{created.Id}",
            new { title = "Updated Title", description = "Updated Desc", isComplete = true });
        var updated = await response.Content.ReadFromJsonAsync<TaskItem>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("Updated Desc", updated.Description);
        Assert.True(updated.IsComplete);
    }

    [Fact]
    public async Task Update_UnknownId_Returns404()
    {
        var response = await _client.PutAsJsonAsync($"/tasks/{Guid.NewGuid()}",
            new { title = "x", description = (string?)null, isComplete = false });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204()
    {
        var created = await PostTaskAsync("To Delete");

        var response = await _client.DeleteAsync($"/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await _client.DeleteAsync($"/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
