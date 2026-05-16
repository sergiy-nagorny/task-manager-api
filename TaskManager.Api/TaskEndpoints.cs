using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Api;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks").WithTags("Tasks");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);

        return app;
    }

    internal static Ok<IEnumerable<TaskItem>> GetAll(TaskRepository repo) =>
        TypedResults.Ok(repo.GetAll());

    internal static Results<Ok<TaskItem>, NotFound> GetById(Guid id, TaskRepository repo) =>
        repo.GetById(id) is TaskItem task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    internal static Created<TaskItem> Create(CreateTaskRequest request, TaskRepository repo)
    {
        var task = repo.Create(request);
        return TypedResults.Created($"/tasks/{task.Id}", task);
    }

    internal static Results<Ok<TaskItem>, NotFound> Update(Guid id, UpdateTaskRequest request, TaskRepository repo) =>
        repo.Update(id, request) is TaskItem task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    internal static Results<NoContent, NotFound> Delete(Guid id, TaskRepository repo) =>
        repo.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}
