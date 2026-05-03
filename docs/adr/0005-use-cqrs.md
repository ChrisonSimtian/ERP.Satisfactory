# 5. Use CQRS in the application layer

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The application has two clearly different shapes of operation:
- **Reads:** browse the recipe catalogue, view a computed production plan, list saved
  plans. These benefit from denormalised, query-shaped models.
- **Writes:** define a production goal, recompute a plan, persist a saved factory.
  These need strict invariants, validation, and domain events.

Mixing both into a single repository / service surface tends to push read models into
write models and vice versa, making each side worse.

## Decision

Use **CQRS** in `ERP.Application`. Commands and queries are separate types; each has a
single handler. Handlers are dispatched via Wolverine (see
[ADR-0006](0006-use-wolverine-as-mediator.md)).

Suggested layout inside `ERP.Application`:

```
ERP.Application/
  Commands/
    PlanProduction/
      PlanProductionCommand.cs
      PlanProductionHandler.cs
  Queries/
    GetRecipes/
      GetRecipesQuery.cs
      GetRecipesHandler.cs
```

We are **not** introducing event sourcing or separate read/write databases at this
stage — only the in-process command/query split.

## Alternatives considered

- **Single service-class-per-aggregate.** Simpler initially, but conflates read and
  write paths and tends to bloat as features land.
- **Full CQRS with read-side projections.** Premature for v1; revisit if/when query
  performance demands it.

## Consequences

- Each request from a host goes through a Wolverine `IMessageBus.InvokeAsync(...)`.
- Validation lives close to the command (FluentValidation can be added later if
  hand-rolled validation gets noisy).
- Tests can target a single handler in isolation without spinning up a host.
