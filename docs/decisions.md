# Decisions

Lightweight record of why key choices were made.

---

## Minimal APIs over Controllers

Chosen for simplicity and to learn the modern .NET approach. Controllers add indirection and ceremony that isn't needed for a focused API like this. Minimal APIs also produce cleaner OpenAPI output when using TypedResults.

---

## .NET Aspire for orchestration

Aspire handles running the API locally, wiring up the dashboard, health checks, and telemetry out of the box. It also provides a clean path to adding databases and other resources in later phases without changing how the app is run.

**Note:** Docker/Podman is NOT required unless containerized resources (Postgres, Redis, etc.) are added. SQLite in Phase 8 won't require Docker.

---

## Built-in OpenAPI over Swashbuckle

.NET 9+ includes built-in OpenAPI support. Swashbuckle is the legacy choice — no reason to add it as an extra dependency on a greenfield project.

## Scalar over Swagger UI

Scalar is a modern, cleaner alternative to Swagger UI for browsing and testing APIs. Added via the `Scalar.AspNetCore` package.

---

## Problem Details (RFC 7807)

All error responses return structured JSON instead of plain strings. Enabled via `AddProblemDetails` + `UseExceptionHandler` + `UseStatusCodePages`. Consumers get consistent, machine-readable errors.

---

## SQLite for Phase 12 (over SQL Server / Postgres)

Keeps the local dev experience simple — no Docker, no server process. Easy to swap for Azure SQL in Phase 16 (cloud deploy) by changing the EF Core connection string and provider.

---

## VS Code + vscode-solution-explorer extension

C# Dev Kit's built-in Solution Explorer has widespread bugs with not appearing reliably. The community extension `vscode-solution-explorer` by Fernando Escolan works independently and reliably adds a dedicated .sln browser to the Activity Bar.

---

## Hosting: all-Azure free tier (Phase 16+)

For free, publicly accessible hosting aligned with the existing Aspire + .NET stack:

| Layer | Choice | Free tier |
|---|---|---|
| Blazor WASM frontend | **Azure Static Web Apps** | 100 GB bandwidth/month, free SSL + custom domain, auto-generated GitHub Actions workflow |
| `TaskManager.Api` | **Azure Container Apps (Consumption)** | 180k vCPU-sec + 400k GiB-sec + 2M requests/month — native Aspire integration |
| Container images | **GitHub Container Registry (GHCR)** | Free with the repo; auth via `GITHUB_TOKEN` |
| Database (Phase 12+) | **Azure SQL Database (Free offer)** | 32 GB, 100k vCore-sec/month, 10 hours/day active |
| CI/CD | **GitHub Actions** | Unlimited minutes for public repos |
| Auth (Phase 15) | **Microsoft Entra ID Free** | 50k MAU |
| Observability | **Application Insights** | 5 GB/month telemetry ingestion |

**Total cost:** $0/month at hobby scale. First paid component is Service Bus (Phase 20, optional).

**Why all-Azure (not Cloudflare Pages + Google Cloud Run, which is cheaper globally):**
Single portal, single bill, single identity model, direct Aspire compatibility (`azd up`). The vendor sprawl of a mixed stack isn't worth the marginal performance gain at this scale.

**Why GHCR over Azure Container Registry:**
ACR Basic is ~$5/month — not free. GHCR is free with the repo, uses `GITHUB_TOKEN` for auth from GitHub Actions, and Container Apps pulls from it without setup friction.

---

## Messaging: Event Grid + Functions over Service Bus (Phase 17–19)

**Service Bus has no free tier.** For learning event-driven patterns at $0/month:
- **Event Grid** (100k ops/month free) — for fire-and-forget domain events
- **Storage Queue** (within Storage free tier) — for background work decoupling
- **Azure Functions Consumption** (1M req/month free) — as the handler runtime
- **Logic Apps Consumption** (4k actions/month free) — for low-code orchestration

This gives the same architectural patterns (publish/subscribe, queues, handlers, workflows) as a Service Bus topology — only the broker differs. **Service Bus is reserved for Phase 20** as an optional paid upgrade for learning DLQ, sessions, and transactions.

**Why this ordering:** the value is in learning event-driven *patterns*, not the specific broker. Defer the paid service until the patterns are familiar.
