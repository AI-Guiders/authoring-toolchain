# vscode-catalog

VS Code / Cursor extension for `.catalog` files.

## Dev

```powershell
# from repo root
./scripts/publish-language-server.ps1 -Configuration Debug
cd extensions/vscode-catalog
npm install
```

Press F5 in VS Code (launch config TBD) or install unpacked from this folder.

Server DLL lands in `server/` (gitignored). Run publish script after LSP changes.
