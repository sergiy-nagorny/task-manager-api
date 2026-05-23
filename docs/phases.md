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

## Phase 6 — Blazor WASM Frontend ✅

**Done.**

- Added `TaskManager.Web.Blazor` (Blazor WebAssembly + MudBlazor 9.x)
- MudBlazor Material Design UI — no JavaScript, one NuGet dependency
- Full CRUD: create form (Enter key support), task table with status chip, complete/undo/edit/delete per row
- `EditTaskDialog` component for inline editing via `PUT /tasks/{id}`
- Snackbar notifications for all mutations
- API URL configured via `wwwroot/appsettings.json` (Blazor WASM runs in browser, can't use Aspire service discovery)
- Registered in Aspire — single F5 starts API + Blazor together
- VS Code launch config: `blazorwasm` debug type for standalone preview without Aspire

---

## Phase 7 — Blazor Component Tests (bUnit) ✅

**Done.**

- Added `TaskManager.Web.Blazor.Tests` project (bUnit 1.37.7 + RichardSzalay.MockHttp)
- `BlazorTestContext` base class: JSInterop.Loose, MockHttp, MudServices + all MudBlazor providers
- `HomeTests` (8 tests): load, create, toggle-complete, delete, error snackbars, disabled-state
- `EditTaskDialogTests` (6 tests): pre-fill, save PUT → close, API error/down snackbars, disabled-when-empty, cancel
- 14 Blazor tests + 38 API tests = 52 passing

> **Future:** E2E browser tests (Playwright) — visual regression, real browser interactions across the full stack.

---

## Phase 8 — Code Quality ✅

**Done.**

- Added `.editorconfig`: CRLF, UTF-8, 4-space C# indent, file-scoped namespaces, var style, nullable diagnostics promoted to errors
- Verified `<Nullable>enable</Nullable>` across all 9 projects
- Ran `dotnet format` to normalise line endings and fix whitespace; all 52 tests pass

---

## Phase 9 — Input Validation ← current

Reject bad input at the API boundary.

- Title required, non-empty, max length
- Return `400 Bad Request` with Problem Details on validation failure
- Cover validation in existing integration tests

---

## Phase 10 — Structured Logging

Make the API observable via explicit logging.

- Inject `ILogger` into endpoint handlers
- Log key operations (task created, not found, deleted, etc.)
- Verify logs appear in Aspire dashboard

---

## Phase 11 — .http File Polish

Improve the developer experience for manual testing.

- Add sample request bodies for all endpoints
- Cover happy path and error cases (missing ID, bad input)

---

## Phase 12 — EF Core + SQLite

Replace the in-memory `TaskRepository` with a real database.

- Add EF Core + SQLite provider
- Define `AppDbContext`
- Migrate `TaskRepository` → EF Core queries
- Add and apply initial migration
- Data persists across restarts

> **Carry-forward from Phase 5:** Update `DiagnosticsEndpoints.Ready()` to use `dbContext.Database.CanConnectAsync()` instead of `repo.GetAll()` — once a real DB is in place, `GetAll()` would enumerate all rows on every probe.

---

## Phase 13 — Filtering + Pagination

Make `GET /tasks` production-grade.

- Filter by `IsComplete`
- Pagination (page + pageSize query params)
- Update integration tests

---

## Phase 14 — API Versioning

Introduce versioning for future-proofing.

- Route-based versioning (`/api/v1/tasks`)
- Document versioning strategy

---

## Phase 15 — Auth (Entra ID)

Protect endpoints with Entra ID bearer tokens.

- Register app in Azure portal
- Add auth middleware
- Protect all task endpoints

---

## Phase 16 — Deploy to free-tier Azure with GitHub Actions CI/CD

Make the existing API + Blazor app accessible over the public web at **$0/month**, with GitHub Actions auto-deploying on every push to `master`.

### Hosting decisions and alternatives

| Component | Choice | Why | Alternatives considered (and why not) |
|---|---|---|---|
| **Blazor WASM frontend** | **Azure Static Web Apps (Free)** | Designed for SPAs; auto-generates GitHub Actions workflow; free SSL + custom domain; SPA routing fallback built-in | *Cloudflare Pages* — fastest CDN but adds a non-Azure vendor; *GitHub Pages* — needs `<base href>` setup; *Netlify/Vercel* — more JS-focused, less Microsoft alignment |
| **TaskManager.Api** | **Azure Container Apps (Consumption)** | Native Aspire integration (`azd up` deploys whole solution); generous monthly free grant; aligns with the existing roadmap | *App Service Free F1* — simpler but 60-min/day CPU cap and 20-min sleep; *Google Cloud Run* — 2M req/month free, but cross-vendor; *Functions* — would require restructuring the API |
| **Container registry** | **GitHub Container Registry (GHCR)** | Free with the repo; auth via `GITHUB_TOKEN`; no extra account | *Docker Hub* — free public repos but separate account; *Azure Container Registry Basic* — ~$5/month, not free |
| **Database** (Phase 12+) | **Azure SQL Database (Free offer)** | Free tier released 2024 (32 GB, 100k vCore-sec/month); SQL stays in Azure | *SQLite file in container* — lost on restart; *Cosmos DB free* — NoSQL, requires API rewrite; *Turso/PlanetScale* — cross-vendor |
| **CI/CD** | **GitHub Actions** | Free unlimited for public repos; deep Azure integration via `azure/login` action | *Azure DevOps Pipelines* — only 1,800 free min/month; *CircleCI* — cross-vendor |
| **Auth** (Phase 15) | **Microsoft Entra ID Free** | Chosen in Phase 15; 50k MAU free | *Auth0* — 7k MAU free; *Firebase Auth* — cross-vendor |
| **Observability** | **Application Insights** (5 GB/month free) | Aspire OTLP exporter targets it natively | *Datadog* — 5 hosts only on free; *Grafana Cloud* — generous but cross-vendor |
| **Domain name** | `*.azurestaticapps.net` + `*.azurecontainerapps.io` (free subdomains) | Zero cost; upgrade later if branded URL matters | Buy a domain (~$10/year from Cloudflare/Namecheap) |

### Deliverables

1. **Containerize the API** — multi-stage `Dockerfile` in `TaskManager.Api`, SDK image → ASP.NET runtime image, targeting `linux/amd64`
2. **GitHub Actions: API CI/CD** — workflow that runs `dotnet test`, builds the Docker image, pushes to GHCR, deploys to Container Apps via `azure/container-apps-deploy-action`
3. **GitHub Actions: frontend CI/CD** — auto-generated by Static Web Apps; builds Blazor WASM and publishes `wwwroot` on each push
4. **Configure production CORS allowlist** — replace `AllowAnyOrigin()` with the Static Web App origin (resolves Phase 6 carry-forward)
5. **Configure frontend API URL** — point `wwwroot/appsettings.Production.json` at the Container Apps FQDN
6. **Verify end-to-end** — open the Static Web App URL, push a trivial change to `master`, watch both pipelines deploy within ~3 minutes

> **Aspire-specific shortcut:** `azd up` will provision Container Apps + log analytics + Application Insights + GHCR registry from the Aspire AppHost automatically. Use it for the first deploy, switch to GitHub Actions for ongoing CD.

> **Carry-forward resolved:** Phase 6's `AllowAnyOrigin` is replaced as part of step 4 above.

---

## Phase 17 — Outbound events with Event Grid + Functions

Introduce event-driven patterns by publishing domain events from the API and processing them in a serverless handler.

- Add `IEventPublisher` interface in `TaskManager.Application`
- Implement `EventGridPublisher` in `TaskManager.Infrastructure` (uses `Azure.Messaging.EventGrid` SDK)
- Publish `TaskCreated` and `TaskCompleted` events from the relevant handlers
- Create `TaskManager.Functions` — Azure Functions project subscribing to the Event Grid topic
- Handler writes structured logs to Application Insights for each event
- Verify end-to-end: POST a task → event appears in handler logs within seconds

**Cost:** $0 — Event Grid free tier (100k ops/month) + Functions Consumption free tier (1M req/month).

---

## Phase 18 — Background work with Storage Queue + Function

Decouple long-running work from the HTTP request path.

- Add `POST /tasks/import` endpoint accepting a list of `CreateTaskRequest`
- Endpoint enqueues one Storage Queue message per task, returns `202 Accepted` with a job ID
- Add a queue-triggered Function in `TaskManager.Functions` that reads one message at a time and creates the task via the existing repository
- Demonstrates: HTTP returns immediately, work happens in the background, automatic retry on failure, poison-message handling via dead-letter queue

**Cost:** $0 — within the Storage transactions free tier.

---

## Phase 19 — Logic App workflow (task-due reminders)

Low-code orchestration across services.

- Add `DueAt` (nullable `DateTime`) to `TaskItem` (requires Phase 12 DB migration)
- Create a Logic App (Consumption) that:
  - Runs hourly via Recurrence trigger
  - Calls `GET /tasks` filtered by `dueAt < now + 1 day AND !isComplete` (requires Phase 13 filtering)
  - For each due task, sends a notification via Outlook 365 connector or SendGrid (free tier: 100 emails/day)
- Logs each run to Application Insights

**Cost:** $0 — within the Logic Apps free tier (4,000 built-in actions/month).

> **Phase ordering note:** depends on Phase 12 (real DB) and Phase 13 (filtering). Attempt earlier only with hardcoded data.

---

## Phase 20 — (Optional, paid) Upgrade to Service Bus

Replace Event Grid + Storage Queue with Azure Service Bus to learn enterprise messaging semantics.

- Replace `EventGridPublisher` with `ServiceBusPublisher`
- Convert the Event Grid topic into a Service Bus **topic** with two subscriptions:
  - `notifications` — consumed by the Functions handler from Phase 17
  - `audit` — consumed by a new audit handler that writes every event to a log
- Use SQL filter expressions to route event types to specific subscriptions
- Demonstrate dead-letter queue handling for poison messages
- Demonstrate session-based ordering for related events

**Cost:** Service Bus Basic ~$0.05/million operations — a few cents/month at hobby scale. **First phase that costs money** — skip if free-tier matters more than learning enterprise patterns.

> The architectural patterns (publish/subscribe, queues, handlers) are identical to Phase 17/18; Service Bus adds DLQ, sessions, and transactions. Worth doing if Service Bus appears in your day job — otherwise Event Grid is sufficient.
