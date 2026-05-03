# 2. Use Blazor for the UI

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The factory planner needs an interactive UI: list items, define production goals,
display computed plans/graphs. We already chose .NET, so the UI stack should ideally
share the language and component model.

## Decision

Use **Blazor** for the UI, scaffolded from the Aspire starter template. We start with
**Interactive Server** rendering (the default of `aspire-starter`) and will revisit
the render mode (Server / WebAssembly / InteractiveAuto) per page as performance and
offline requirements emerge.

## Alternatives considered

- **React / TypeScript SPA.** Forces a second toolchain and language, fragmenting
  domain models between C# and TS.
- **Razor Pages / MVC.** Adequate for forms-over-data, but interactive planner UI
  benefits from Blazor's component model and SignalR streaming.

## Consequences

- The `Web` project is a Blazor Server app. The `ApiService` project still exists for
  HTTP endpoints used by external clients or eventual Wasm pages.
- Components live under `src/Web/Components/`. Domain logic must not leak into Razor
  components — components call into `ERP.Application` via Wolverine.
