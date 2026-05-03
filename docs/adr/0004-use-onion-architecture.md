# 4. Use Onion Architecture

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The product has a non-trivial domain (factory planning: items, recipes, throughput,
power, logistics). Coupling that domain to UI or persistence concerns will make it
hard to evolve, test, and reuse. We need a layout where the domain stays independent
of frameworks and infrastructure.

## Decision

Adopt **Onion Architecture**. Dependencies flow inward only:

```
Web / ApiService / AppHost   (Presentation / Composition)
            ↓
    ERP.Infrastructure       (adapters, persistence, external data)
            ↓
    ERP.Application          (use cases, CQRS handlers — see ADR-0005)
            ↓
    ERP.Domain               (entities, value objects, domain events)
```

`Satisfactory.Catalog` is a separate module that `ERP.Infrastructure` depends on
(it provides the game-specific catalogue of items/recipes that the generic ERP
domain plans against).

## Alternatives considered

- **Plain layered (n-tier) architecture.** Allows infrastructure types to leak into
  business logic via downward dependencies.
- **Vertical-slice / feature folders.** Tempting and we may layer it inside
  `ERP.Application`, but the *outer* layout still benefits from Onion separation.

## Consequences

- `ERP.Domain` has zero project references and minimal NuGet dependencies — it must
  not pull in EF Core, ASP.NET, or any framework concerns.
- `ERP.Application` depends only on `ERP.Domain` and is where Wolverine handlers live.
- `ERP.Infrastructure` is the only project allowed to depend on databases/files/etc.
- Hosts (`Web`, `ApiService`) compose `Application` + `Infrastructure`; they are
  presentation only.
