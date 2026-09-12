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
| `samples/planet/` | Sample `*.gdlproj` declare entry |
| `build/Platform.Gdl.Emit.*` | MSBuild targets: GDL emit → `Generated/*.g.cs` |
| `samples/GdlEmit.Catalog.Sample/` | Sample `*.csproj` wiring for catalog emit |

## GDL emit (MSBuild)

Planet `*.csproj` files invoke per-quarry emit before compile (or fail CI when `Generated/*.g.cs` is stale). SSOT targets live in `build/Platform.Gdl.Emit.*` (packaged as `Authoring.Toolchain.Build`):

```xml
<Import Project="path/to/authoring-toolchain/build/Platform.Gdl.Emit.props" />

<PropertyGroup>
  <GdlEmitInput>authoring/planet.catalog.gdl</GdlEmitInput>
  <GdlEmitKind>catalog</GdlEmitKind>
  <GdlEmitNamespace>Planet.Generated</GdlEmitNamespace>
  <GdlEmitClass>PlanetCatalog</GdlEmitClass>
  <GdlEmitOutputDir>$(MSBuildProjectDirectory)\Generated</GdlEmitOutputDir>
</PropertyGroup>
```

| Property | Role |
|----------|------|
| `GdlEmitInput` | `*.catalog.gdl` or `*.deck.gdl` source |
| `GdlEmitKind` | `catalog` (`authoring emit`) or `deck` (`deck emit`) |
| `GdlEmitNamespace` / `GdlEmitClass` | Passed to emit CLI |
| `GdlEmitOutputDir` | Output folder (default `Generated/`) |
| `GdlEmitForce` | `true` to re-emit regardless of timestamps |

`BeforeCompile` runs emit when the input is newer than `$(GdlEmitOutputDir)/$(GdlEmitClass).g.cs`. CI stale check:

```powershell
dotnet msbuild Planet.csproj -p:Configuration=Release -t:GdlEmitVerify
```

Deck wiring: same properties with `GdlEmitKind=deck` and `GdlEmitDeckTool` / `GdlEmitDeckCommand` pointing at [AIGuiders.DotnetTools.DeckEmit](https://github.com/AI-Guiders/guiders-assist).

## `gdlc`

Unified declare-time front-end per [GUIDERS-ADR-0059 §10](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0059-gdl-hyperlane.md#10-compiler-pipeline-gdlc). Routes single-file or `*.gdlproj` project `emit` to existing per-quarry stacks; `validate` / `sat` come later.

```powershell
dotnet tool run --project src/Gdlc.Cli gdlc emit --lang=cs samples/catalog/dash.catalog.gdl --namespace Dash.Generated --class DashCatalog
dotnet tool run --project src/Gdlc.Cli gdlc emit --lang=cs samples/deck/dashspec-studio.deck.gdl --namespace Dash.Generated --out Generated/DeckIds.g.cs
dotnet tool run --project src/Gdlc.Cli gdlc emit --lang=cs --project samples/planet/planet.gdlproj --out Generated
dotnet tool run --project src/Gdlc.Cli gdlc --help
```

| Suffix | Delegates to |
|--------|----------------|
| `*.catalog.gdl` | `authoring emit` stack (catalog codegen) |
| `*.deck.gdl` | `deck emit` stack (deck codegen) |

### `*.gdlproj` (wave 2)

Minimal declare entry mapped to `AuthoringProject` ([GUIDERS-ADR-0051](https://github.com/AI-Guiders/guiders-platform/blob/main/docs/adr/GUIDERS-ADR-0051-authoring-project-abstraction.md)). `document` paths are relative to the `.gdlproj` directory; workspace root is the common ancestor of the project file and all listed documents.

```text
# samples/planet/planet.gdlproj
project planet

document ../catalog/dash.catalog.gdl
document ../deck/dashspec-studio.deck.gdl
```

`gdlc emit --project` iterates listed documents and writes `*.g.cs` under `--out` (default: `<gdlproj-dir>/Generated/`). Class names derive from the document stem (`dash.catalog.gdl` → `DashCatalog`, `dashspec-studio.deck.gdl` → `DashspecStudioDeckIds`).

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
