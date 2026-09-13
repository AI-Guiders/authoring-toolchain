# ATC-ADR-0003: gdlc sat — Hoare observers via SatObserverRegistry

| Field | Value |
|-------|-------|
| **Status** | Accepted |
| **Date** | 2026-09-13 |
| **Scope** | `authoring-toolchain`, `guiders-platform` (SAT-001 Epic E) |

## Context

Wave 2 `gdlc sat` routed only through quarry plugins (`IGdlQuarryPlugin.Sat`). Catalog and deck quarries return *skipped* until config quarry ships ([GUIDERS-ADR-0064](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0064-config-gdl-quarry-family.md)).

Platform SAT-001 introduces `SatObserverRegistry` and Hoare-style observers for ADR facts, config contracts, and IDE-session gates ([GUIDERS-FSHARP-ADR-0006](https://github.com/AI-Guiders/guiders-fsharp/blob/main/docs/adr/GUIDERS-FSHARP-ADR-0006-adr-lifecycle-verifiable-facts.md), [GUIDERS-FSHARP-ADR-0007](https://github.com/AI-Guiders/guiders-fsharp/blob/main/docs/adr/GUIDERS-FSHARP-ADR-0007-open-build-ssot-ftc-correspondence.md)).

## Decision

1. **`AIGuiders.Platform.Authoring.Sat.Abstractions`** owns `SatContext`, `ISatObserver`, and `SatObserverRegistry`.
2. **`Gdlc.Plugins.Federation`** bootstraps sat observers via `GdlSatBootstrap` (parallel to `GdlPluginBootstrap`):
   - Always registers `QuarryPluginSatObserver` (`quarry.plugin`) for `*.catalog.gdl` / `*.deck.gdl`.
   - Reflectively loads platform observer assemblies when present (`Sat.AdrFacts`, `Sat.ConfigContract`, `Sat.IdeSessionGates`).
   - Falls back to `AdrFactsSatObserverStub` until `Sat.AdrFacts` ships.
3. **`gdlc sat`** parses `SatOptions`:
   - `--adr=<ID>` and/or `--facts=<path>` → Hoare observer path (`SatObserverRegistry.TryObserve`).
   - Otherwise → quarry plugin path via registry (same semantics as plugin `.Sat`).
   - `--project` runs observers per document and aggregates supported/skipped counts.
4. **Exit codes** unchanged: `0` ok · `1` failed · `2` usage/registry miss · `3` skipped.

## Consequences

- Federation gains a reference to `Sat.Abstractions` now; optional observer packages wire in without `Gdlc.Cli` changes.
- `gdlc sat --adr=GUIDERS-FSHARP-ADR-0007` is the pilot CLI for ADR fact verification.
- Quarry sat and Hoare sat coexist; priority and `CanObserve` disambiguate context.

## Non-goals

- ADR facts parser implementation (platform `Sat.AdrFacts` assembly).
- Config quarry IR sat (blocked on ADR-0064).
