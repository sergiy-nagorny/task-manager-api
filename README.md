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

### Run without Aspire (direct API debug)

To run or debug just the API project without the Aspire dashboard:

```bash
dotnet run --project TaskManager.Api
```

URLs when running directly:
- HTTP: `http://localhost:5181`
- HTTPS: `https://localhost:7296`

In **VS Code**, press `F5` and select the `TaskManager.Api` launch profile (requires C# Dev Kit extension).

#### Debugging with breakpoints in VS Code

1. Click in the gutter (left of the line number) to set a breakpoint — a red dot appears
2. Press `F5` to start debugging (or **Run → Start Debugging**)
3. Make an API request (via Scalar UI, `.http` file, or curl)
4. VS Code pauses at the breakpoint — use the debug toolbar to:
   - `F10` — step over (next line)
   - `F11` — step into (go inside a method call)
   - `F5` — continue to next breakpoint
   - Hover over variables to inspect their values
5. Press `Shift+F5` to stop the debugger

### API docs

Once running, open the Scalar UI:

```
https://localhost:7296/scalar/v1
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
