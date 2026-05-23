# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the full solution via Aspire (dashboard + API)
aspire run                                # preferred (Aspire CLI 13.3+)
dotnet run --project TaskManager.AppHost  # equivalent fallback

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

Seven projects in the solution:

- **TaskManager.AppHost** — Aspire entry point. Run this for local dev. Registers `TaskManager.Api` as a child process. Add databases/services here in later phases.
- **TaskManager.Api** — The REST API. Endpoints + DI wiring only.
- **TaskManager.Domain** — Core entities (`TaskItem`). No dependencies.
- **TaskManager.Application** — Interfaces (`ITaskRepository`) and DTOs (`CreateTaskRequest`, `UpdateTaskRequest`). Depends on Domain only.
- **TaskManager.Infrastructure** — Implementations (`InMemoryTaskRepository`). Phase 10 replaces this with EF Core + SQLite.
- **TaskManager.Api.Tests** — xUnit tests: unit tests for handlers/repository, integration tests via `WebApplicationFactory`.
- **TaskManager.ServiceDefaults** — Shared Aspire boilerplate (OpenTelemetry, health checks). Call `builder.AddServiceDefaults()` in each service project. Rarely touched directly.

## API project structure

- `TaskEndpoints.cs` — Minimal API route handlers for `/tasks` CRUD, all `internal static`
- `DiagnosticsEndpoints.cs` — `GET /ping` (liveness) and `GET /ready` (readiness) handlers
- `Program.cs` — service registration and middleware pipeline

## Key patterns

**Minimal API handlers** are `internal static` methods on `TaskEndpoints` / `DiagnosticsEndpoints` so they can be called directly in unit tests without spinning up a full HTTP server. The test project accesses them via `InternalsVisibleTo` in `TaskManager.Api.csproj`.

**TypedResults** — handlers declare their full return type (e.g. `Results<Ok<TaskItem>, NotFound>`). Don't switch to `IResult` — this is intentional for accurate OpenAPI response schemas.

**Error handling** — `AddProblemDetails` + `UseExceptionHandler` + `UseStatusCodePages` in `Program.cs` ensures all errors return RFC 7807 Problem Details JSON. Don't add manual error response shapes.

**Current data store** is in-memory (`InMemoryTaskRepository` in `TaskManager.Infrastructure`). Phase 10 replaces this with EF Core + SQLite — when that happens, `InMemoryTaskRepository` is removed and `AppDbContext` takes over.

## Testing approach

Unit tests call handlers and repository methods directly — no HTTP, no `WebApplicationFactory`. Integration tests use `WebApplicationFactory<Program>` to test the full HTTP stack end-to-end.

Run a focused test by class name:
```bash
dotnet test TaskManager.Api.Tests --filter "ClassName~TaskRepositoryTests"
```

## Phases

See `docs/phases.md` for the full roadmap. Current phase is tracked there. Don't implement features from future phases.
