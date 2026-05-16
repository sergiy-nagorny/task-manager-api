using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Application;
using TaskManager.Domain;

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

    internal static Ok<IEnumerable<TaskItem>> GetAll(ITaskRepository repo) =>
        TypedResults.Ok(repo.GetAll());

    internal static Results<Ok<TaskItem>, NotFound> GetById(Guid id, ITaskRepository repo) =>
        repo.GetById(id) is TaskItem task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    internal static Created<TaskItem> Create(CreateTaskRequest request, ITaskRepository repo)
    {
        var task = repo.Create(request);
        return TypedResults.Created($"/tasks/{task.Id}", task);
    }

    internal static Results<Ok<TaskItem>, NotFound> Update(Guid id, UpdateTaskRequest request, ITaskRepository repo) =>
        repo.Update(id, request) is TaskItem task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    internal static Results<NoContent, NotFound> Delete(Guid id, ITaskRepository repo) =>
        repo.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}
