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

## Phase 2 — EF Core + SQLite

Replace the in-memory `TaskRepository` with a real database.

- Add EF Core + SQLite provider
- Define `AppDbContext`
- Migrate `TaskRepository` → EF Core queries
- Add and apply initial migration
- Data persists across restarts

---

## Phase 3 — Validation + Filtering / Pagination

- Input validation (e.g. title required, max length)
- Filter tasks by `IsComplete`
- Pagination on `GET /tasks`

---

## Phase 4 — Auth (Entra ID)

- Protect endpoints with Entra ID (Azure AD) bearer tokens
- Register app in Azure portal
- Add auth middleware

---

## Phase 5 — Containerize + Deploy to Azure Container Apps

- Add Dockerfile
- Switch from SQLite to Azure SQL
- Deploy via Azure Container Apps
- Wire up Aspire for cloud resources
