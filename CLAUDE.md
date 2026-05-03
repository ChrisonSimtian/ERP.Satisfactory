# ERP.Satisfactory — Claude guidance

This repository is an ERP-style production-planning system for the game *Satisfactory*.
The number-one objective is **helping the user plan a factory based on the inputs they
have and the outputs they need.**

## Architecture

All architecturally significant decisions live in [`docs/adr/`](docs/adr/README.md) as
Architecture Decision Records. Read the ADR index before making structural changes —
these decisions are the source of truth for *why* the codebase looks the way it does.

If you make a new architecturally significant decision, **add a new ADR** (copy
[`0000-template.md`](docs/adr/0000-template.md), give it the next sequential number,
and update the index in `docs/adr/README.md`). Never silently change a decision recorded
in an ADR — supersede it with a new one.

Quick summary of the load-bearing decisions (see ADRs for context):

- [.NET 10](docs/adr/0001-use-dotnet-10.md), [Blazor](docs/adr/0002-use-blazor-for-ui.md),
  [.NET Aspire](docs/adr/0003-use-dotnet-aspire-for-orchestration.md) for orchestration.
- [Onion Architecture](docs/adr/0004-use-onion-architecture.md) +
  [CQRS](docs/adr/0005-use-cqrs.md) in the application layer.
- [Wolverine](docs/adr/0006-use-wolverine-as-mediator.md) is the in-process mediator —
  **do not reach for MediatR**.
- [Backlog on GitHub](docs/adr/0007-track-backlog-on-github.md): epics → milestones,
  user stories → issues.

## Repository layout

```
src/
  AppHost/              .NET Aspire orchestrator
  ApiService/           Backend HTTP API (composes Application + Infrastructure)
  Web/                  Blazor frontend (composes Application + Infrastructure)
  ServiceDefaults/      Shared Aspire host defaults
  ERP/                  The standalone ERP/planning module (Onion layers below)
    Domain/             ERP.Domain      — entities, value objects, domain events
    Application/        ERP.Application — CQRS handlers dispatched via Wolverine
    Infrastructure/     ERP.Infrastructure — persistence, adapters
  Satisfactory/         Game-specific module — ERP "piggybacks" on this for game data
    Catalog/            Satisfactory.Catalog — items, recipes, buildings catalogue
test/
  ERP/
    Domain.Tests/
    Application.Tests/
docs/
  adr/                  Architecture Decision Records
```

**Folder/namespace convention:** folder names drop the full namespace prefix. The
parent folder denotes the bounded context (e.g. `src/ERP/Domain` →
`ERP.Domain.csproj` → namespace `ERP.Domain`). If we ever add a CRM module, it lives
in `src/CRM/...` with namespace `CRM.*`.

## Onion dependency rules

- `ERP.Domain` depends on **nothing**.
- `ERP.Application` depends on `ERP.Domain`.
- `ERP.Infrastructure` depends on `ERP.Application` (+ Domain + `Satisfactory.Catalog`).
- Hosts (`Web`, `ApiService`) depend on `ERP.Application` + `ERP.Infrastructure`.
- `AppHost` depends on `ApiService` + `Web` (Aspire orchestration only).

When adding new code, place it in the **innermost** layer that satisfies its
dependencies. Domain logic must not leak into Infrastructure or Web.

## Backlog

Work is tracked on GitHub:
- **Epics** are GitHub milestones.
- **User stories** are GitHub issues, assigned to the matching milestone.

When introducing new work, prefer creating an issue over piling things into a single
session.

## Build & run

```powershell
dotnet build ERP.Satisfactory.sln
dotnet run --project src/AppHost
```
