# 0013. Map visualiser approach

- Status: Proposed
- Date: 2026-05-11
- Deciders: Chris

## Context

Once live factory state is parsed ([ADR 0012](0012-live-factory-state-via-node-sidecar.md)),
the planner can show *where* miners, smelters, and belts actually sit in the
game world. A map visualiser is the natural surface — both to confirm "yes,
this is your factory" and to highlight ore-starved modules, belt breaches, or
unrouted spare capacity.

The candidate visualisers:

- `SC-InteractiveMap` (the engine behind satisfactory-calculator.com) is the
  obvious choice on features, but its license explicitly forbids forking or
  self-deployment.
- `ficsit-felix` is MIT but archived since April 2022; it won't load v1.2 saves
  without significant rework.
- Rolling our own with **Leaflet** over coordinates derived from the parsed
  save is the only path that avoids both the license trap and the maintenance
  cliff.

Map tile art is Coffee Stain IP and must not be redistributed.

## Decision

Build a thin map view in Blazor using **Leaflet** via JSInterop. The map is
fed by GeoJSON rendered server-side from parsed save state. Tile layer starts
as a **procedural / abstract topographic layer** generated from public
resource-node coordinates only — no copyrighted Coffee Stain tile art is
shipped. A user-supplied local tile cache may be supported later as an
optional feature.

## Alternatives considered

- **iframe `satisfactory-calculator.com`.** Zero dev effort, but no overlay
  control, no deep-link from a domain entity to its on-map marker, and a
  third-party dependency on the navigation path. Useful only as an "open
  externally" link, not as the primary UX.
- **Fork `SC-InteractiveMap`.** License forbids it.
- **Server-side render to PNG / SVG via SkiaSharp.** No interactivity. Useful
  later as a snapshot-export feature, not as the primary visualiser.
- **Ship the in-game map tiles.** Coffee Stain IP. Rejected outright.
- **3D map via `three.js` / BabylonJS.** Significantly more effort and unclear
  payoff over a clean 2D top-down view. Defer.

## Consequences

- Blazor takes a small JSInterop dependency on Leaflet (~40 KB gzipped). 2D
  only; tile resolution is bounded by our own tile generator.
- The map page is **read-only at v1** — display the factory, don't edit it.
  Editing capabilities (plan a new module on the map, simulate a belt reroute)
  are an explicit later epic.
- Resource node coordinates and shapes are derivable from the parsed save
  (`BP_ResourceNode_C` and `BP_ResourceDeposit_C`) plus published wiki data.
  No proprietary data needed.
- Performance: 459 resource nodes plus hundreds of buildings sit well inside
  Leaflet's comfort zone; clustering is unnecessary at v1 zoom levels.
- The GeoJSON contract is what the front-end consumes, so swapping Leaflet for
  MapLibre, OpenLayers, or a 3D viewer later is a front-end-only change.
