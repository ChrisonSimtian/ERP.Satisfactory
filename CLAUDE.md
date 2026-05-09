# ERP.Satisfactory

ERP-style production planner for the game *Satisfactory*. Primary objective:
**help the user plan a factory based on the inputs they have and the outputs they need.**

## Where things live

- **Architecture decisions** — [`docs/adr/`](docs/adr/README.md). Read the index before
  making structural changes; supersede with a new ADR when introducing one.
- **Project conventions for Claude** — [`.claude/`](.claude/README.md): repo layout,
  onion rules, build & run, custom agents (incl. the in-game **ADA** assistant).
- **Backlog** — GitHub: epics = milestones, user stories = issues.

## Build & run

```powershell
dotnet build ERP.Satisfactory.sln
dotnet run --project src/AppHost
```
