# Phases

Incremental build plan — start simple, evolve one concern at a time.

---

## Phase 1 — Minimal API + In-Memory Store ✅

**Done.**

- Minimal API project with full CRUD for tasks
- In-memory Dictionary as data store (resets on restart)
- OpenAPI spec + Scalar UI
- Problem Details error responses
- .NET Aspire (AppHost + ServiceDefaults)
- Git + GitHub set up

---

## Phase 2 — Unit Tests ✅

**Done.**

- Add `TaskManager.Api.Tests` project (xUnit)
- Unit test `TaskRepository` (Create, GetAll, GetById, Update, Delete)
- Unit test endpoint handlers directly (happy path + not found cases)

---

## Phase 3 — Integration Tests ✅

**Done.**

Test the full HTTP stack end-to-end.

- Use `WebApplicationFactory<Program>` to spin up the real API in-process
- Test each endpoint over HTTP (status codes, response bodies)
- No mocking — real in-memory store, real middleware pipeline

---

## Phase 4 — Clean Architecture Split ✅

**Done.**

- Extracted `TaskManager.Domain` (entity: `TaskItem`)
- Extracted `TaskManager.Application` (interface: `ITaskRepository`, DTOs: `CreateTaskRequest`, `UpdateTaskRequest`)
- Extracted `TaskManager.Infrastructure` (impl: `InMemoryTaskRepository`)
- `TaskManager.Api` slimmed to endpoints + DI wiring only
- All 27 tests pass on the new structure

---

## Phase 5 — Diagnostic Endpoints ✅

**Done.**

- `GET /ping` — liveness check, always 200 if the process is running
- `GET /ready` — readiness check, exercises each key dependency and returns a `checks` array with per-resource status (200 healthy / 503 degraded)
- Both endpoints available in all environments (not dev-only) for container health probes
- 38 tests passing

---

## Phase 6 — Code Quality ← current

Enforce consistent style and safety across the solution.

- Add `.editorconfig` with formatting rules (indentation, line endings, etc.)
- Verify nullable reference types are enforced (`<Nullable>enable</Nullable>`)
- Fix any resulting warnings

---

## Phase 7 — Input Validation

Reject bad input at the API boundary.

- Title required, non-empty, max length
- Return `400 Bad Request` with Problem Details on validation failure
- Cover validation in existing integration tests

---

## Phase 8 — Structured Logging

Make the API observable via explicit logging.

- Inject `ILogger` into endpoint handlers
- Log key operations (task created, not found, deleted, etc.)
- Verify logs appear in Aspire dashboard

---

## Phase 9 — .http File Polish

Improve the developer experience for manual testing.

- Add sample request bodies for all endpoints
- Cover happy path and error cases (missing ID, bad input)

---

## Phase 10 — EF Core + SQLite

Replace the in-memory `TaskRepository` with a real database.

- Add EF Core + SQLite provider
- Define `AppDbContext`
- Migrate `TaskRepository` → EF Core queries
- Add and apply initial migration
- Data persists across restarts

> **Carry-forward from Phase 5:** Update `DiagnosticsEndpoints.Ready()` to use `dbContext.Database.CanConnectAsync()` instead of `repo.GetAll()` — once a real DB is in place, `GetAll()` would enumerate all rows on every probe.

---

## Phase 11 — Filtering + Pagination

Make `GET /tasks` production-grade.

- Filter by `IsComplete`
- Pagination (page + pageSize query params)
- Update integration tests

---

## Phase 12 — API Versioning

Introduce versioning for future-proofing.

- Route-based versioning (`/api/v1/tasks`)
- Document versioning strategy

---

## Phase 13 — Auth (Entra ID)

Protect endpoints with Entra ID bearer tokens.

- Register app in Azure portal
- Add auth middleware
- Protect all task endpoints

---

## Phase 14 — Containerize + Deploy to Azure Container Apps

- Add Dockerfile
- Switch from SQLite to Azure SQL
- Deploy via Azure Container Apps
- Wire up Aspire for cloud resources
