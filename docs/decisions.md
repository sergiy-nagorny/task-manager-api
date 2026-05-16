# Decisions

Lightweight record of why key choices were made.

---

## Minimal APIs over Controllers

Chosen for simplicity and to learn the modern .NET approach. Controllers add indirection and ceremony that isn't needed for a focused API like this. Minimal APIs also produce cleaner OpenAPI output when using TypedResults.

---

## .NET Aspire for orchestration

Aspire handles running the API locally, wiring up the dashboard, health checks, and telemetry out of the box. It also provides a clean path to adding databases and other resources in later phases without changing how the app is run.

**Note:** Docker/Podman is NOT required unless containerized resources (Postgres, Redis, etc.) are added. SQLite in Phase 2 won't require Docker.

---

## Built-in OpenAPI over Swashbuckle

.NET 9+ includes built-in OpenAPI support. Swashbuckle is the legacy choice — no reason to add it as an extra dependency on a greenfield project.

## Scalar over Swagger UI

Scalar is a modern, cleaner alternative to Swagger UI for browsing and testing APIs. Added via the `Scalar.AspNetCore` package.

---

## Problem Details (RFC 7807)

All error responses return structured JSON instead of plain strings. Enabled via `AddProblemDetails` + `UseExceptionHandler` + `UseStatusCodePages`. Consumers get consistent, machine-readable errors.

---

## SQLite for Phase 2 (over SQL Server / Postgres)

Keeps the local dev experience simple — no Docker, no server process. Easy to swap for Azure SQL in Phase 5 by changing the EF Core connection string and provider.

---

## VS Code + vscode-solution-explorer extension

C# Dev Kit's built-in Solution Explorer has widespread bugs with not appearing reliably. The community extension `vscode-solution-explorer` by Fernando Escolan works independently and reliably adds a dedicated .sln browser to the Activity Bar.
