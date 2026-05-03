# 6. Use Wolverine as the in-process mediator

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

CQRS (ADR-0005) requires an in-process bus to dispatch commands/queries to handlers.
The .NET community default is MediatR, but MediatR's licensing change in 2024 and
Wolverine's stronger feature set (transactional outbox, scheduling, message routing,
runtime code-gen for handler dispatch) make it a better long-term bet.

## Decision

Use **Wolverine** (`WolverineFx` on NuGet) as the in-process mediator. **Do not use
MediatR.**

- `builder.Host.UseWolverine();` is called in each host (`ApiService`, `Web`).
- Handlers live in `ERP.Application` and are discovered by Wolverine's
  convention-based scanning (handler classes with `Handle` / `Consume` methods on
  command/query types).
- Hosts call `IMessageBus.InvokeAsync<TResponse>(query)` for queries and
  `IMessageBus.InvokeAsync(command)` for commands.

## Alternatives considered

- **MediatR.** Familiar, but the commercial licensing pivot makes it a poor default
  for a long-lived project. Smaller feature surface than Wolverine.
- **Hand-rolled dispatcher.** Avoids a dependency but loses Wolverine's handler
  pipelines, code-gen, and future outbox/scheduling support we expect to want.

## Consequences

- The `WolverineFx` package must be referenced from `ERP.Application` and from any
  host project that calls `UseWolverine`.
- Handler discovery is by convention — keep handler classes in
  `ERP.Application/Commands/...` or `ERP.Application/Queries/...`.
- When we eventually need durable messaging (sagas, outbox), we extend Wolverine
  rather than adopting a second library.
