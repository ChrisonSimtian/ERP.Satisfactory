# 7. Track the backlog on GitHub (epics = milestones, stories = issues)

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The work needs a single source of truth that lives near the code, supports linking
PRs to work items, and is free for public/personal projects. Standalone trackers
(Jira, Linear, Notion) add an extra surface to keep in sync.

## Decision

Track the backlog on **GitHub**:

- **Epics → GitHub milestones.** One milestone per epic; the milestone description
  captures the goal and acceptance summary.
- **User stories → GitHub issues.** Each story is an issue assigned to its parent
  epic's milestone. Use issue templates and labels for `story`, `bug`, `chore`, etc.

PRs reference issues by `Closes #N` so the backlog auto-closes on merge.

## Alternatives considered

- **GitHub Projects (boards).** Useful complement, but milestones already model
  epics natively; we can layer Projects on top later if we need a kanban view.
- **External tracker (Jira / Linear).** Overkill for the size of this project and
  fragments the workflow.

## Consequences

- New work starts as a GitHub issue; do not add planning artefacts to the repo
  itself.
- New epics are created with `gh api repos/:owner/:repo/milestones`; new stories with
  `gh issue create --milestone "Name"`.
- The README/CLAUDE.md should keep pointers to the milestone view rather than
  duplicating roadmap content.
