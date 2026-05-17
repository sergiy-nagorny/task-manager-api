using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Application;

namespace TaskManager.Api;

public static class DiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/").WithTags("Diagnostics");

        group.MapGet("/ping", Ping);
        group.MapGet("/ready", Ready);

        return app;
    }

    internal static Ok<DiagnosticsResponse> Ping() =>
        TypedResults.Ok(new DiagnosticsResponse("ok"));

    internal static Results<Ok<ReadinessResponse>, JsonHttpResult<ReadinessResponse>> Ready(ITaskRepository repo)
    {
        var checks = new List<CheckResult>();

        try
        {
            repo.GetAll();
            checks.Add(new CheckResult("task-store", "ok"));
        }
        catch
        {
            checks.Add(new CheckResult("task-store", "degraded"));
        }

        var allOk = checks.All(c => c.Status == "ok");
        var response = new ReadinessResponse(allOk ? "ok" : "degraded", checks);

        return allOk
            ? TypedResults.Ok(response)
            : TypedResults.Json(response, statusCode: 503);
    }
}

internal record DiagnosticsResponse(string Status);
internal record CheckResult(string Name, string Status);
internal record ReadinessResponse(string Status, IReadOnlyList<CheckResult> Checks);
