namespace TaskManager.Api.Tests;

public class TaskEndpointsTests
{
    private readonly ITaskRepository _repo = new InMemoryTaskRepository();

    // GET /tasks

    [Fact]
    public void GetAll_EmptyRepo_ReturnsEmptyList()
    {
        var result = TaskEndpoints.GetAll(_repo);

        Assert.Empty(result.Value!);
    }

    [Fact]
    public void GetAll_WithTasks_ReturnsAllTasks()
    {
        _repo.Create(new CreateTaskRequest("Task A", null));
        _repo.Create(new CreateTaskRequest("Task B", null));

        var result = TaskEndpoints.GetAll(_repo);

        Assert.Equal(2, result.Value!.Count());
    }

    // GET /tasks/{id}

    [Fact]
    public void GetById_ExistingId_ReturnsOkWithTask()
    {
        var created = _repo.Create(new CreateTaskRequest("Find me", null));

        var result = TaskEndpoints.GetById(created.Id, _repo);

        var ok = Assert.IsType<Ok<TaskItem>>(result.Result);
        Assert.Equal(created.Id, ok.Value!.Id);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNotFound()
    {
        var result = TaskEndpoints.GetById(Guid.NewGuid(), _repo);

        Assert.IsType<NotFound>(result.Result);
    }

    // POST /tasks

    [Fact]
    public void Create_ValidRequest_ReturnsCreatedWithTask()
    {
        var result = TaskEndpoints.Create(new CreateTaskRequest("New task", "desc"), _repo);

        Assert.NotNull(result.Value);
        Assert.Equal("New task", result.Value.Title);
        Assert.Equal("/tasks/" + result.Value.Id, result.Location);
    }

    // PUT /tasks/{id}

    [Fact]
    public void Update_ExistingId_ReturnsOkWithUpdatedTask()
    {
        var created = _repo.Create(new CreateTaskRequest("Original", null));

        var result = TaskEndpoints.Update(created.Id, new UpdateTaskRequest("Updated", null, true), _repo);

        var ok = Assert.IsType<Ok<TaskItem>>(result.Result);
        Assert.Equal("Updated", ok.Value!.Title);
        Assert.Null(ok.Value.Description);
        Assert.True(ok.Value.IsComplete);
    }

    [Fact]
    public void Update_UnknownId_ReturnsNotFound()
    {
        var result = TaskEndpoints.Update(Guid.NewGuid(), new UpdateTaskRequest("X", null, false), _repo);

        Assert.IsType<NotFound>(result.Result);
    }

    // DELETE /tasks/{id}

    [Fact]
    public void Delete_ExistingId_ReturnsNoContent()
    {
        var created = _repo.Create(new CreateTaskRequest("Delete me", null));

        var result = TaskEndpoints.Delete(created.Id, _repo);

        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public void Delete_UnknownId_ReturnsNotFound()
    {
        var result = TaskEndpoints.Delete(Guid.NewGuid(), _repo);

        Assert.IsType<NotFound>(result.Result);
    }
}
