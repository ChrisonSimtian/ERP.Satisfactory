# 3. Use .NET Aspire for orchestration

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The solution is already multi-project (Web, ApiService, ServiceDefaults) and will grow
to include databases and possibly background workers. Local-dev orchestration,
service discovery, and consistent telemetry/health-check defaults are repetitive
boilerplate.

## Decision

Use **.NET Aspire** (currently `Aspire.AppHost.Sdk/13.2.4`). The `AppHost` project is
the entry point for local development; `ServiceDefaults` provides shared
OpenTelemetry, health-check, and resilience configuration to every service.

## Alternatives considered

- **Plain `docker-compose`.** Works, but loses C#-native service discovery, telemetry
  wiring, and the dashboard.
- **Hand-rolled multi-project launchSettings.** No service-to-service discovery, no
  shared defaults.

## Consequences

- `dotnet run --project src/AppHost` is the canonical way to run the system locally.
- Adding a new service means adding a `ProjectReference` from `AppHost` and
  `builder.AddProject<Projects.X>("name")` in `AppHost.cs`.
- Resources like databases or message brokers are added via Aspire integration
  packages, not standalone Compose files.
