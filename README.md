# ERP.Satisfactory

A .NET 10 / Blazor / Aspire application that helps you plan factories in the game
[*Satisfactory*](https://www.satisfactorygame.com/) given the inputs you have and the
outputs you need.

## Run it

Requires a .NET 10 SDK (RC2 or later — see `global.json`).

```powershell
dotnet build ERP.Satisfactory.sln
dotnet run --project src/AppHost
```

The Aspire dashboard URL will be printed in the console output.

## Architecture

- **Onion architecture** with CQRS handlers dispatched via Wolverine.
- **Two bounded contexts**: `ERP` (the standalone planner) and `Satisfactory` (the
  game-specific catalogue the planner reads).
- **.NET Aspire** orchestrates local dev across the API service, Blazor UI, and
  service defaults.

All architecturally significant decisions are recorded in [`docs/adr/`](docs/adr/README.md).

## Backlog

- Epics → [GitHub milestones](https://github.com/ChrisonSimtian/ERP.Satisfactory/milestones)
- User stories → [GitHub issues](https://github.com/ChrisonSimtian/ERP.Satisfactory/issues)

## Contributing

See [`CLAUDE.md`](CLAUDE.md) for the full project conventions, layout, and dependency
rules.
