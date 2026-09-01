# Extensions

VS Code / Cursor extensions. Each grammar gets a folder `vscode-<grammar>/`.

| Extension | Grammar | LSP |
|-----------|---------|-----|
| [vscode-catalog](vscode-catalog/) | `.catalog` | `Authoring.LanguageServer.Catalog` |

Publish flow: `./scripts/publish-language-server.ps1` then `vsce package` in extension folder.
