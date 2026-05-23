namespace TaskManager.Api.Tests;

public class TaskRepositoryTests
{
    private readonly ITaskRepository _repo = new InMemoryTaskRepository();

    [Fact]
    public void Create_AddsTaskAndReturnsIt()
    {
        var task = _repo.Create(new CreateTaskRequest("Buy milk", null));

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("Buy milk", task.Title);
        Assert.False(task.IsComplete);
    }

    [Fact]
    public void GetAll_ReturnsAllCreatedTasks()
    {
        _repo.Create(new CreateTaskRequest("Task A", null));
        _repo.Create(new CreateTaskRequest("Task B", null));

        var tasks = _repo.GetAll().ToList();

        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsTask()
    {
        var created = _repo.Create(new CreateTaskRequest("Find me", null));

        var found = _repo.GetById(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var result = _repo.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void Update_ExistingId_UpdatesAndReturnsTask()
    {
        var created = _repo.Create(new CreateTaskRequest("Original", null));

        var updated = _repo.Update(created.Id, new UpdateTaskRequest("Updated", "desc", true));

        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Title);
        Assert.Equal("desc", updated.Description);
        Assert.True(updated.IsComplete);
    }

    [Fact]
    public void Update_UnknownId_ReturnsNull()
    {
        var result = _repo.Update(Guid.NewGuid(), new UpdateTaskRequest("X", null, false));

        Assert.Null(result);
    }

    [Fact]
    public void Delete_ExistingId_ReturnsTrueAndRemovesTask()
    {
        var created = _repo.Create(new CreateTaskRequest("Delete me", null));

        var deleted = _repo.Delete(created.Id);

        Assert.True(deleted);
        Assert.Null(_repo.GetById(created.Id));
    }

    [Fact]
    public void Delete_UnknownId_ReturnsFalse()
    {
        var result = _repo.Delete(Guid.NewGuid());

        Assert.False(result);
    }
}
