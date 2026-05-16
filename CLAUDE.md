# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the full solution via Aspire (dashboard + API)
dotnet run --project TaskManager.AppHost

# Run the API directly (no Aspire dashboard)
dotnet run --project TaskManager.Api

# Build
dotnet build TaskManager.sln

# Run all tests
dotnet test TaskManager.Api.Tests

# Run a single test
dotnet test TaskManager.Api.Tests --filter "FullyQualifiedName~GetById_ExistingId"
```

## Architecture

Four projects in the solution:

- **TaskManager.AppHost** — Aspire entry point. Run this for local dev. Registers `TaskManager.Api` as a child process. Add databases/services here in later phases.
- **TaskManager.Api** — The REST API. All business logic lives here.
- **TaskManager.Api.Tests** — xUnit unit tests for `TaskRepository` and `TaskEndpoints`.
- **TaskManager.ServiceDefaults** — Shared Aspire boilerplate (OpenTelemetry, health checks). Call `builder.AddServiceDefaults()` in each service project. Rarely touched directly.

## API project structure

- `TaskItem.cs` — data model + `CreateTaskRequest` / `UpdateTaskRequest` records
- `TaskRepository.cs` — in-memory `Dictionary<Guid, TaskItem>` store, registered as a `Singleton`
- `TaskEndpoints.cs` — Minimal API route handlers, all `internal static` methods grouped under `/tasks`
- `Program.cs` — service registration and middleware pipeline

## Key patterns

**Minimal API handlers** are `internal static` methods on `TaskEndpoints` so they can be called directly in unit tests without spinning up a full HTTP server. The test project accesses them via `InternalsVisibleTo` in `TaskManager.Api.csproj`.

**TypedResults** — handlers declare their full return type (e.g. `Results<Ok<TaskItem>, NotFound>`). Don't switch to `IResult` — this is intentional for accurate OpenAPI response schemas.

**Error handling** — `AddProblemDetails` + `UseExceptionHandler` + `UseStatusCodePages` in `Program.cs` ensures all errors return RFC 7807 Problem Details JSON. Don't add manual error response shapes.

**Current data store** is in-memory (`TaskRepository`). Phase 8 replaces this with EF Core + SQLite — when that happens, `TaskRepository` is removed and `AppDbContext` takes over.

## Testing approach

Unit tests call handlers and repository methods directly — no HTTP, no `WebApplicationFactory`. Integration tests (Phase 3) will cover the full HTTP stack.

Run a focused test by class name:
```bash
dotnet test TaskManager.Api.Tests --filter "ClassName~TaskRepositoryTests"
```

## Phases

See `docs/phases.md` for the full roadmap. Current phase is tracked there. Don't implement features from future phases.
