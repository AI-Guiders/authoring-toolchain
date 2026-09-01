# ATC-ADR-0001: Authoring toolchain boundary

| | |
|---|---|
| **Status** | Accepted |
| **Date** | 2026-09-01 |
| **Tags** | #authoring #toolchain #lsp #dx #monorepo |
| **Related** | [GUIDERS-ADR-0048](https://github.com/AI-Guiders/guiders-platform/blob/main/_wip-adr-0048/GUIDERS-ADR-0048-authoring-quarry-family.md) · [GUIDERS-ADR-0047](https://github.com/AI-Guiders/guiders-platform/blob/main/_wip-adr-0047/GUIDERS-ADR-0047-command-for-doi.md) · [GUIDERS-ADR-0025](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0025-language-intelligence-boundary.md) · DASHSPEC `editor/` |

## Context

Authoring DSLs (`.catalog`, `.dashspec`, …) need more than a parser:

```text
grammar → diagnostics → LSP → VSIX → CLI validate → conformance CI → learn labs
```

`guiders-platform` stays **headless** ([GUIDERS-ADR-0001](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0001-platform-boundary.md)): NuGet only, no npm. DashSpec already hosts planet LSP under `dash-spec/editor/`. Federation cross-planet tooling does not belong inside platform or inside every planet repo.

Operator scale: small human team + agents. Agents read ADR + parser; humans need LSP, tutorials, and atomic grammar/tooling releases.

## Decision

### 1. New monorepo: `authoring-toolchain`

Sibling to `guiders-platform`. Owns **editor and CI surfaces** for federation authoring grammars.

### 2. Split

| Concern | Repo | Artifact |
|---------|------|----------|
| Parser, AST, `Authoring.Core` kit | `guiders-platform` | `AIGuiders.Platform.Authoring.*` NuGet |
| LSP host, VSIX shell, `dotnet tool` | `authoring-toolchain` | extension + CLI |
| Conformance vectors (grammar) | `guiders-platform` (`Authoring.Conformance`) | JSON vectors |
| Conformance runner (CI harness) | `authoring-toolchain` | test project / script |
| Planet product DSL | planet repo | `.dashspec` sovereign |
| Learn path (labs) | `authoring-toolchain/docs/learn` | markdown |

### 3. First wave

1. `Authoring.LanguageServer.Catalog` — thin LSP over `Authoring.Command.Catalog` (when published).
2. `extensions/vscode-catalog` — syntax + LSP client.
3. `Authoring.Cli validate` — same diagnostics as LSP for CI.
4. `Authoring.Toolchain.Host` — shared workspace-root + diagnostic mapping.

DashSpec `vscode-dashspec` **not migrated** in wave 1; may adopt `Authoring.Toolchain.Host` when API stabilizes.

### 4. Release coupling

Grammar change in platform → bump NuGet ref in toolchain → publish extension + CLI in same PR when possible. Platform remains releasable without toolchain; toolchain pins minimum `Authoring.*` version.

## Consequences

- +1 repo to maintain; CI scoped to tooling only.
- Clear home for npm / VSIX without polluting platform.
- Agent playbook can point to one toolchain repo for «add LSP feature».

## Open

| # | Item |
|---|------|
| 1 | Publish `vscode-catalog` to Open VSX / marketplace |
| 2 | Shared TextMate kit vs per-grammar syntax |
| 3 | Move dash-spec LSP shared bits — only after catalog LSP proves kit |
