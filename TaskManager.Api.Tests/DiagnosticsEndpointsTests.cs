namespace TaskManager.Api.Tests;

public class DiagnosticsEndpointsTests
{
    private readonly ITaskRepository _repo = new InMemoryTaskRepository();

    // GET /ping

    [Fact]
    public void Ping_ReturnsOk()
    {
        var result = DiagnosticsEndpoints.Ping();

        Assert.IsType<Ok<DiagnosticsResponse>>(result);
    }

    [Fact]
    public void Ping_ReturnsStatusOk()
    {
        var result = DiagnosticsEndpoints.Ping();

        Assert.Equal("ok", result.Value!.Status);
    }

    // GET /ready

    [Fact]
    public void Ready_HealthyRepo_ReturnsOk()
    {
        var result = DiagnosticsEndpoints.Ready(_repo);

        Assert.IsType<Ok<ReadinessResponse>>(result.Result);
    }

    [Fact]
    public void Ready_HealthyRepo_StatusIsOk()
    {
        var result = DiagnosticsEndpoints.Ready(_repo);

        var ok = Assert.IsType<Ok<ReadinessResponse>>(result.Result);
        Assert.Equal("ok", ok.Value!.Status);
    }

    [Fact]
    public void Ready_HealthyRepo_ChecksContainTaskStore()
    {
        var result = DiagnosticsEndpoints.Ready(_repo);

        var ok = Assert.IsType<Ok<ReadinessResponse>>(result.Result);
        var check = Assert.Single(ok.Value!.Checks);
        Assert.Equal("task-store", check.Name);
        Assert.Equal("ok", check.Status);
    }

    [Fact]
    public void Ready_ThrowingRepo_Returns503()
    {
        var result = DiagnosticsEndpoints.Ready(new ThrowingRepository());

        var json = Assert.IsType<JsonHttpResult<ReadinessResponse>>(result.Result);
        Assert.Equal(503, json.StatusCode);
    }

    [Fact]
    public void Ready_ThrowingRepo_ChecksShowDegraded()
    {
        var result = DiagnosticsEndpoints.Ready(new ThrowingRepository());

        var json = Assert.IsType<JsonHttpResult<ReadinessResponse>>(result.Result);
        Assert.Equal("degraded", json.Value!.Status);
        var check = Assert.Single(json.Value.Checks);
        Assert.Equal("task-store", check.Name);
        Assert.Equal("degraded", check.Status);
    }

    private sealed class ThrowingRepository : ITaskRepository
    {
        public IEnumerable<TaskItem> GetAll() => throw new InvalidOperationException("db unavailable");
        public TaskItem? GetById(Guid id) => throw new NotImplementedException();
        public TaskItem Create(CreateTaskRequest request) => throw new NotImplementedException();
        public TaskItem? Update(Guid id, UpdateTaskRequest request) => throw new NotImplementedException();
        public bool Delete(Guid id) => throw new NotImplementedException();
    }
}
