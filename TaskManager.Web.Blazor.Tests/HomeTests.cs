using System.Net;
using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RichardSzalay.MockHttp;
using TaskManager.Web.Blazor.Models;
using TaskManager.Web.Blazor.Pages;

public class HomeTests : BlazorTestContext
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);
    private static string Json<T>(T value) => JsonSerializer.Serialize(value, JsonOpts);

    private TaskItem MakeTask(string title = "Buy milk", bool done = false) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        IsComplete = done,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public void LoadTasks_ShowsTasksFromApi()
    {
        var task = MakeTask("Buy milk");
        MockHttp.When("http://localhost/tasks").Respond("application/json", Json(new[] { task }));

        var cut = RenderComponent<Home>();

        cut.WaitForAssertion(() => Assert.Contains("Buy milk", cut.Markup));
    }

    [Fact]
    public void LoadTasks_EmptyList_ShowsEmptyMessage()
    {
        MockHttp.When("http://localhost/tasks").Respond("application/json", "[]");

        var cut = RenderComponent<Home>();

        cut.WaitForAssertion(() => Assert.Contains("No tasks yet", cut.Markup));
    }

    [Fact]
    public void LoadTasks_ApiDown_ShowsSnackbarError()
    {
        MockHttp.When("http://localhost/tasks").Throw(new HttpRequestException("network error"));

        var cut = RenderComponent<Home>();

        SnackbarProvider.WaitForAssertion(() =>
            Assert.Contains("Could not reach the API", SnackbarProvider.Markup));
    }

    [Fact]
    public async Task CreateTask_PostsToApiAndAppendsRow()
    {
        var newTask = MakeTask("New task");
        MockHttp.When(HttpMethod.Get, "http://localhost/tasks").Respond("application/json", "[]");
        MockHttp.When(HttpMethod.Post, "http://localhost/tasks").Respond("application/json", Json(newTask));

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Contains("No tasks yet", cut.Markup));

        cut.Find("input").Change("New task");
        var addBtn = cut.FindAll("button").First(b => b.TextContent.Contains("Add Task"));
        await addBtn.ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() => Assert.Contains("New task", cut.Markup));
    }

    [Fact]
    public void CreateTask_AddButtonDisabled_WhenTitleEmpty()
    {
        MockHttp.When("http://localhost/tasks").Respond("application/json", "[]");

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Contains("No tasks yet", cut.Markup));

        var addBtn = cut.FindAll("button").First(b => b.TextContent.Contains("Add Task"));
        Assert.True(addBtn.HasAttribute("disabled"));
    }

    [Fact]
    public async Task ToggleComplete_PutsToApiAndUpdatesStatus()
    {
        var task = MakeTask("Buy milk", done: false);
        var doneTask = new TaskItem
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsComplete = true,
            CreatedAt = task.CreatedAt
        };
        MockHttp.When(HttpMethod.Get, "http://localhost/tasks")
            .Respond("application/json", Json(new[] { task }));
        MockHttp.When(HttpMethod.Put, $"http://localhost/tasks/{task.Id}")
            .Respond("application/json", Json(doneTask));

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Contains("Buy milk", cut.Markup));

        // Toggle button is the first icon button (index 0 per row)
        var toggleBtn = cut.FindComponents<MudIconButton>()[0].Find("button");
        await toggleBtn.ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() => Assert.Contains("Done", cut.Markup));
    }

    [Fact]
    public async Task DeleteTask_DeletesFromApiAndRemovesRow()
    {
        var task = MakeTask("Buy milk");
        MockHttp.When(HttpMethod.Get, "http://localhost/tasks")
            .Respond("application/json", Json(new[] { task }));
        MockHttp.When(HttpMethod.Delete, $"http://localhost/tasks/{task.Id}")
            .Respond(HttpStatusCode.NoContent);

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Contains("Buy milk", cut.Markup));

        // Delete button is the third icon button (index 2 per row)
        var deleteBtn = cut.FindComponents<MudIconButton>()[2].Find("button");
        await deleteBtn.ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() => Assert.DoesNotContain("Buy milk", cut.Markup));
    }

    [Fact]
    public async Task DeleteTask_ApiError_RefreshesFromApi()
    {
        var task = MakeTask("Buy milk");
        MockHttp.Expect(HttpMethod.Get, "http://localhost/tasks")
            .Respond("application/json", Json(new[] { task }));
        MockHttp.Expect(HttpMethod.Delete, $"http://localhost/tasks/{task.Id}")
            .Respond(HttpStatusCode.InternalServerError);
        MockHttp.Expect(HttpMethod.Get, "http://localhost/tasks")
            .Respond("application/json", "[]");

        var cut = RenderComponent<Home>();
        cut.WaitForAssertion(() => Assert.Contains("Buy milk", cut.Markup));

        var deleteBtn = cut.FindComponents<MudIconButton>()[2].Find("button");
        await deleteBtn.ClickAsync(new MouseEventArgs());

        cut.WaitForAssertion(() => MockHttp.VerifyNoOutstandingExpectation());
    }
}
