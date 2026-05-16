# Architecture

## Overview

A Task Manager REST API built with .NET 10, using Minimal APIs and .NET Aspire for orchestration. Learning project designed to evolve incrementally through defined phases.

## Solution Structure

```
TaskManager.sln
├── TaskManager.AppHost          — Aspire orchestrator (entry point for running locally)
├── TaskManager.Api              — The REST API
└── TaskManager.ServiceDefaults  — Shared Aspire config (telemetry, health checks)
```

## Stack

| Concern | Choice |
|---|---|
| Framework | .NET 10 Minimal APIs |
| Orchestration | .NET Aspire |
| API docs | Built-in OpenAPI + Scalar UI |
| Error responses | Problem Details (RFC 7807) |
| Data store (Phase 1) | In-memory Dictionary |
| Data store (Phase 2) | EF Core + SQLite |
| Auth (Phase 4) | Entra ID |
| Hosting (Phase 5) | Azure Container Apps |

## API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | /tasks | List all tasks |
| GET | /tasks/{id} | Get a task by ID |
| POST | /tasks | Create a task |
| PUT | /tasks/{id} | Update a task |
| DELETE | /tasks/{id} | Delete a task |

## Key Design Choices

- **Minimal APIs** — no controllers; routes map directly to static handler methods in `TaskEndpoints.cs`
- **TypedResults** — handler return types are explicit (e.g. `Results<Ok<TaskItem>, NotFound>`), giving accurate OpenAPI response schemas without annotations
- **Singleton TaskRepository** — survives between requests; resets on process restart (replaced by EF Core in Phase 2)
- **Problem Details everywhere** — `AddProblemDetails` + `UseExceptionHandler` + `UseStatusCodePages` ensures all errors return structured JSON
- **Aspire for local dev** — AppHost starts the API as a child process and provides the dashboard; no Docker needed for plain .NET resources

## Local URLs (when running)

- Aspire dashboard: `https://localhost:17027` (port may vary)
- API: listed as a resource in the Aspire dashboard
- Scalar UI: `<api-base-url>/scalar/v1`
- OpenAPI spec: `<api-base-url>/openapi/v1.json`
