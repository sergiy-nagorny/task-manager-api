# Task Manager API

A REST API for managing tasks, built with .NET 10 Minimal APIs and .NET Aspire. Learning project exploring modern .NET and Azure best practices incrementally.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [GitHub CLI](https://cli.github.com/) (optional, for pushing to GitHub)

### Run locally

```bash
dotnet run --project TaskManager.AppHost
```

The Aspire dashboard will open automatically in your browser. The API is listed there as a resource.

> No Docker or Podman required — SQLite is used as the local database (no container needed).

### API docs

Once running, open the Scalar UI from the API's base URL:

```
<api-base-url>/scalar/v1
```

## Endpoints

| Method | Route | Description |
|---|---|---|
| GET | /tasks | List all tasks |
| GET | /tasks/{id} | Get a task by ID |
| POST | /tasks | Create a task |
| PUT | /tasks/{id} | Update a task |
| DELETE | /tasks/{id} | Delete a task |

## Documentation

- [Architecture](docs/architecture.md) — stack, project structure, design choices
- [Decisions](docs/decisions.md) — why key choices were made
- [Phases](docs/phases.md) — roadmap and progress
- [Commands](docs/commands.md) — all CLI commands used in this project
