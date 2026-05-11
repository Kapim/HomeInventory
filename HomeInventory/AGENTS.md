# AGENTS.md

Guidance for coding agents working in this repository.

## Project Overview

HomeInventory is a .NET solution organized into server, client, shared contracts, and tests.

- Solution file: `HomeInventory.slnx`
- Server projects: `HomeInventory.Api`, `HomeInventory.Application`, `HomeInventory.Domain`, `HomeInventory.Infrastructure`
- Client projects: `HomeInventory.Client`, `HomeInventory.Desktop.Wpf`, `HomeInventory.Mobile.Maui`
- Shared contracts: `HomeInventory.Contracts`
- Tests: `HomeInventory.*.Tests`

The repository currently targets .NET 10.0, with WPF using `net10.0-windows`.

## Common Commands

Run from the repository root:

```powershell
dotnet restore HomeInventory.slnx
dotnet build HomeInventory.slnx
dotnet test HomeInventory.slnx
```

For focused test runs:

```powershell
dotnet test HomeInventory.Domain.Tests\HomeInventory.Domain.Tests.csproj
dotnet test HomeInventory.Infrastructure.Tests\HomeInventory.Infrastructure.Tests.csproj
dotnet test HomeInventory.Desktop.Wpf.Tests\HomeInventory.Desktop.Wpf.Tests.csproj
dotnet test HomeInventory.Client.Tests\HomeInventory.Client.Tests.csproj
```

## Development Notes

- Preserve existing user changes. Check `git status --short` before editing and do not revert unrelated work.
- Keep changes scoped to the project or feature being modified.
- Follow the existing architecture: domain behavior in `HomeInventory.Domain`, application orchestration in `HomeInventory.Application`, persistence and external services in `HomeInventory.Infrastructure`, API surface in `HomeInventory.Api`, and UI concerns in the client projects.
- Use nullable-aware C# and keep implicit usings consistent with the project files.
- Prefer existing patterns and dependencies already present in the target project.
- For WPF work, keep view logic in view models where practical and use `CommunityToolkit.Mvvm` patterns already referenced by the project.
- Add or update tests when changing domain logic, infrastructure behavior, shared contracts, or view-model behavior.

## Verification

Before handing off code changes, run the narrowest relevant test project. For cross-project or shared-contract changes, run:

```powershell
dotnet test HomeInventory.slnx
```

If tests cannot be run in the current environment, report the exact command that should be run and why it was skipped.
