# ATC-ADR-0002: gdlc plugin registry (vFINAL)

| Field | Value |
|-------|-------|
| **Status** | Accepted |
| **Date** | 2026-09-13 |
| **Scope** | `authoring-toolchain`, `guiders-platform`, `guiders-wpf` |

## Context

Wave 2 `gdlc` routed emit in-process by hard-coded quarry switches and linked all codegen assemblies into `Gdlc.Cli`. MSBuild still invoked legacy `authoring emit` and `deck emit` for single-file targets ([GUIDERS-ADR-0065](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0065-gdl-emit-operational-paths.md)).

## Decision

1. **`AIGuiders.Platform.Authoring.Emit.Abstractions`** defines `IGdlQuarryPlugin`, request/result DTOs, and `GdlQuarryRegistry` keyed by **quarry + lang + surface**.
2. **Quarry plugins** ship in their owning packages:
   - `CatalogQuarryPlugin` → `Platform.Execution.CommandPlane.Catalog.CodeGen`
   - `DeckQuarryPlugin` → `Surface.Wpf.CodeGen` (`surface=wpf`)
3. **`Gdlc.Plugins.Federation`** bootstraps built-in plugins; `Gdlc.Cli` is a thin orchestrator (emit · validate · sat).
4. **MSBuild** single-file and project emit both invoke **`gdlc` only** (`Platform.Gdl.Emit.targets`).
5. **`*.gdlproj`** may declare defaults: `emit lang cs`, `emit surface wpf`.
6. **`gdlc sat`** routes through plugins; catalog/deck return *skipped* until config quarry ([ADR-0064](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0064-config-gdl-quarry-family.md)) ships observers.

## Consequences

- New surfaces add a `Surface.*.CodeGen` plugin package + federation registration — no `Gdlc.Cli` rebuild for quarry logic.
- `guiders-assist/AIGuiders.DotnetTools.DeckEmit` is deprecated for MSBuild; escape hatch only.
- Tool version **1.0.0** — registry contract is stable; new quarries extend plugins, not CLI switches.

## Non-goals

- `emit --lang=toml|json|fs` back-ends (future plugins).
- Config quarry `sat` implementation (blocked on ADR-0064 IR).
