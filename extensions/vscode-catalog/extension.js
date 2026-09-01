const path = require("path");
const { LanguageClient, TransportKind } = require("vscode-languageclient/node");

/** @type {LanguageClient | undefined} */
let client;

/**
 * @param {import('vscode').ExtensionContext} context
 */
function activate(context) {
  const serverDll = path.join(
    context.extensionPath,
    "server",
    "Authoring.LanguageServer.Catalog.dll"
  );

  client = new LanguageClient(
    "guidersCatalog",
    "Guiders Catalog Language Server",
    {
      run: { command: "dotnet", args: [serverDll], transport: TransportKind.stdio },
      debug: { command: "dotnet", args: [serverDll], transport: TransportKind.stdio },
    },
    {
      documentSelector: [{ scheme: "file", language: "catalog" }],
    }
  );

  context.subscriptions.push(client.start());
}

function deactivate() {
  if (!client) {
    return undefined;
  }

  return client.stop();
}

module.exports = { activate, deactivate };
