# Config quarry samples (ADR-0064 pilot)

## `cdp-newcomer.config.gdl`

Pilot setup profile for CDP newcomer installs. Run contract checks with:

```bash
gdlc sat --workspace <workspace-root> samples/config/cdp-newcomer.config.gdl
```

`<workspace-root>` must contain `agent-notes-mcp.toml` with `[knowledge].primary = "personal"` and a matching `[knowledge.roots].personal` path.

### Contract `hot.personal.l0`

| Role | Predicate | Behavior (SAT P2) |
|------|-----------|-------------------|
| `requires` | `primary_is_personal` | `[knowledge].primary` in `agent-notes-mcp.toml` is `personal`. |
| `ensures` | `hot_l0_sections_present` | F# `ContractPredicates.evaluateHotL0SectionsPresent`: resolves `{personal}` from TOML, reads `sources table` paths (`agent-notes.md`, `memory-architecture-v1.json`), verifies every manifest `l0` id has a `<!-- section:id -->` block in hot notes (above `<!-- public-cut -->` when slice is `above_public_cut`). |

### Facts table (stubs in P2)

| `verified_by` | SAT behavior |
|---------------|--------------|
| `golden:GS-config-hot-l0-newcomer` | Not evaluated by `gdlc sat` yet (golden harness). |
| `install-cdp.personal-seed@*` | Recorded as an informational note only — install commit evidence is not verified in P2. |

Successful run example summary:

```text
sat: ok — cdp-newcomer (primary_is_personal via `.../agent-notes-mcp.toml`; hot_l0_sections_present via `.../agent-notes.md` (...); facts stub: `install-cdp.personal-seed@...` ...)
```
