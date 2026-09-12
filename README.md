# Authoring Toolchain

Federation **authoring DX** monorepo: LSP hosts, VS Code / Cursor extensions, CLI, conformance runners, learn labs.

Grammar, parsers, and typed IR live in **[guiders-platform](https://github.com/AI-Guiders/guiders-platform)** (`AIGuiders.Platform.Authoring.*`). This repo is the **boring glue** that turns parsers into editor and CI surfaces.

> **Early preview (0.x):** scaffolding wave. First consumer: `.catalog` ([GUIDERS-ADR-0047](https://github.com/AI-Guiders/guiders-platform/blob/main/_wip-adr-0047/GUIDERS-ADR-0047-command-for-doi.md)).

## Boundary

```text
guiders-platform (headless NuGet)
  Authoring.Core · Authoring.Command.Catalog · Authoring.Conformance
        │
        ▼
authoring-toolchain (this repo)
  LanguageServer.* · extensions/* · Authoring.Cli · conformance-runner · docs/learn
        │
        ▼
planet repos (sovereign product DSL)
  dash-spec/editor/vscode-dashspec  — may adopt shared kit later
```

| Layer | SSOT repo | Ships |
|-------|-----------|-------|
| Grammar + AST + diagnostics | `guiders-platform` | NuGet |
| LSP + VSIX + validate CLI | `authoring-toolchain` | extension + `dotnet tool` |
| Planet DSL bodies | product repo | `.dashspec`, content |

See [design/ATC-ADR-0001-toolchain-boundary.md](design/ATC-ADR-0001-toolchain-boundary.md).

## Layout

| Path | Role |
|------|------|
| `src/Authoring.Toolchain.Host` | Shared LSP bootstrap helpers |
| `src/Authoring.LanguageServer.Catalog` | `.catalog` language server |
| `src/Authoring.Cli` | `validate` / `format` / `emit` CLI |
| `extensions/vscode-catalog` | VS Code / Cursor extension |
| `conformance/` | Grammar vector runner (CI) |
| `docs/learn/` | Progressive labs for humans |
| `samples/catalog/` | Golden `.catalog` snippets |
| `samples/deck/` | Golden `.deck` snippets |

## `gdlc` (wave-1 stub)

Unified declare-time front-end per [GUIDERS-ADR-0059 §10](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0059-gdl-hyperlane.md#10-compiler-pipeline-gdlc). Wave 1 routes single-file `emit` to existing per-quarry stacks; `*.gdlproj` / `validate` / `sat` come later.

```powershell
dotnet tool run --project src/Gdlc.Cli gdlc emit --lang=cs samples/catalog/dash.catalog.gdl --namespace Dash.Generated --class DashCatalog
dotnet tool run --project src/Gdlc.Cli gdlc emit --lang=cs samples/deck/dashspec-studio.deck.gdl --namespace Dash.Generated --out Generated/DeckIds.g.cs
dotnet tool run --project src/Gdlc.Cli gdlc --help
```

| Suffix | Delegates to |
|--------|----------------|
| `*.catalog.gdl` | `authoring emit` stack (catalog codegen) |
| `*.deck.gdl` | `deck emit` stack (deck codegen) |

Manual smoke (after `dotnet build`):

```powershell
dotnet test tests/Gdlc.Cli.SmokeTests/Gdlc.Cli.SmokeTests.csproj -c Release
```

## Build

```powershell
git clone https://github.com/AI-Guiders/authoring-toolchain.git
cd authoring-toolchain
dotnet build
```

Publish catalog LSP for the extension:

```powershell
./scripts/publish-language-server.ps1 -Configuration Release
```

## Related

- [GUIDERS-ADR-0048 Authoring quarry](https://github.com/AI-Guiders/guiders-platform/blob/main/_wip-adr-0048/GUIDERS-ADR-0048-authoring-quarry-family.md)
- [DashSpec editor tooling](https://github.com/AI-Guiders/dash-spec/tree/main/editor)

Software: [MIT](LICENSE)
