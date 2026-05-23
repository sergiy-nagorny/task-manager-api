using TaskManager.Web.Blazor.Components;
using TaskManager.Web.Blazor.Models;

public class EditTaskDialogTests : BlazorTestContext
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);
    private static string Json<T>(T value) => JsonSerializer.Serialize(value, JsonOpts);

    private readonly IDialogService DialogService;

    public EditTaskDialogTests()
    {
        DialogService = Services.GetRequiredService<IDialogService>();
    }

    private TaskItem MakeTask(string title = "Buy milk", string? description = "2 liters") => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Description = description,
        IsComplete = false,
        CreatedAt = DateTime.UtcNow
    };

    private async Task<IDialogReference> ShowDialog(TaskItem task)
    {
        IDialogReference dialogRef = null!;
        var parameters = new DialogParameters<EditTaskDialog> { { d => d.Task, task } };
        await DialogProvider.InvokeAsync(async () =>
            dialogRef = await DialogService.ShowAsync<EditTaskDialog>("Edit Task", parameters));
        return dialogRef;
    }

    [Fact]
    public async Task OnInit_PreFillsFieldsFromTask()
    {
        var task = MakeTask("Buy milk");
        await ShowDialog(task);

        var input = DialogProvider.Find("input");
        Assert.Equal("Buy milk", input.GetAttribute("value"));
    }

    [Fact]
    public async Task Save_PutsToApi_AndClosesDialog()
    {
        var task = MakeTask("Buy milk");
        var updated = new TaskItem
        {
            Id = task.Id,
            Title = "Buy oat milk",
            Description = task.Description,
            IsComplete = task.IsComplete,
            CreatedAt = task.CreatedAt
        };
        MockHttp.When(HttpMethod.Put, $"http://localhost/tasks/{task.Id}")
            .Respond("application/json", Json(updated));

        var dialogRef = await ShowDialog(task);

        var inputs = DialogProvider.FindAll("input");
        inputs[0].Change("Buy oat milk");

        var saveBtn = DialogProvider.FindAll("button").First(b => b.TextContent.Contains("Save"));
        await saveBtn.ClickAsync(new MouseEventArgs());

        DialogProvider.WaitForAssertion(() => Assert.True(dialogRef.Result.IsCompleted));

        var result = await dialogRef.Result;
        Assert.False(result!.Canceled);
        var returned = result.Data as TaskItem;
        Assert.NotNull(returned);
        Assert.Equal("Buy oat milk", returned!.Title);
    }

    [Fact]
    public async Task Save_ApiError_ShowsSnackbar()
    {
        var task = MakeTask();
        MockHttp.When(HttpMethod.Put, $"http://localhost/tasks/{task.Id}")
            .Respond(HttpStatusCode.InternalServerError);

        await ShowDialog(task);

        var saveBtn = DialogProvider.FindAll("button").First(b => b.TextContent.Contains("Save"));
        await saveBtn.ClickAsync(new MouseEventArgs());

        SnackbarProvider.WaitForAssertion(() =>
            Assert.Contains("Failed to save task", SnackbarProvider.Markup));
    }

    [Fact]
    public async Task Save_ApiDown_ShowsSnackbar()
    {
        var task = MakeTask();
        MockHttp.When(HttpMethod.Put, $"http://localhost/tasks/{task.Id}")
            .Throw(new HttpRequestException("network error"));

        await ShowDialog(task);

        var saveBtn = DialogProvider.FindAll("button").First(b => b.TextContent.Contains("Save"));
        await saveBtn.ClickAsync(new MouseEventArgs());

        SnackbarProvider.WaitForAssertion(() =>
            Assert.Contains("Could not reach the API", SnackbarProvider.Markup));
    }

    [Fact]
    public async Task SaveButton_Disabled_WhenTitleEmpty()
    {
        var task = MakeTask();
        await ShowDialog(task);

        DialogProvider.Find("input").Change("");

        var saveBtn = DialogProvider.FindAll("button").First(b => b.TextContent.Contains("Save"));
        Assert.True(saveBtn.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Cancel_ClosesDialogWithoutSaving()
    {
        var task = MakeTask();
        var dialogRef = await ShowDialog(task);

        var cancelBtn = DialogProvider.FindAll("button").First(b => b.TextContent.Contains("Cancel"));
        await cancelBtn.ClickAsync(new MouseEventArgs());

        DialogProvider.WaitForAssertion(() => Assert.True(dialogRef.Result.IsCompleted));

        var result = await dialogRef.Result;
        Assert.True(result!.Canceled);
        MockHttp.VerifyNoOutstandingRequest();
    }
}
