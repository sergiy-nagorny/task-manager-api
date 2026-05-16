# Commands Used

## Install & Setup
```
winget install Microsoft.DotNet.SDK.10 --accept-source-agreements --accept-package-agreements --silent
winget install GitHub.cli --accept-source-agreements --accept-package-agreements
dotnet dev-certs https --trust
```

## Scaffold the Solution
```
dotnet new install Aspire.ProjectTemplates
dotnet new aspire -n TaskManager -o .
dotnet new webapi -n TaskManager.Api -o TaskManager.Api --use-minimal-apis
dotnet sln TaskManager.sln add TaskManager.Api/TaskManager.Api.csproj
```

## Add References & Packages
```
dotnet add TaskManager.AppHost reference TaskManager.Api
dotnet add TaskManager.Api reference TaskManager.ServiceDefaults
dotnet add TaskManager.Api package Scalar.AspNetCore
```

## Build & Run
```
dotnet build TaskManager.sln
dotnet run --project TaskManager.AppHost   # runs Aspire dashboard + API (no Docker needed for plain .NET resources)
```

## Git & GitHub
```
git config --global user.email "sergiy.nagorny@gmail.com"
git config --global user.name "Sergiy Nagorny"
git init
git add <files>
git commit -m "message"
gh repo create task-manager-api --public --source=. --remote=origin --push
gh auth login                            # authenticate GitHub CLI (run in PowerShell, not via !)
```

## VS Code Extensions
```
# Search in Ctrl+Shift+X:
# vscode-solution-explorer  (by Fernando Escolan) — reliable .sln browser
# C# Dev Kit                (ms-dotnettools.csdevkit)  — IntelliSense, debugging
```

## Claude Code Session Management
```
claude --continue                        # resume most recent session
claude --resume                          # pick from list of past sessions
claude --dangerously-skip-permissions    # skip all approval prompts for the session
```

## Claude Code Settings
```
/config       # open settings UI
/permissions  # manage permission mode
```
