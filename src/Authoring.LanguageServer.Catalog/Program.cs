using Authoring.LanguageServer.Catalog;
using Authoring.LanguageServer.Catalog.Handlers;
using Authoring.Toolchain.Host;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Server;

var server = await LanguageServer.From(options =>
    options
        .WithInput(Console.OpenStandardInput())
        .WithOutput(Console.OpenStandardOutput())
        .WithServices(services => { })
        .WithHandler<CatalogTextDocumentHandler>()
        .OnInitialize((_, request, _) =>
        {
            CatalogServerState.WorkspaceRoots = WorkspaceRoots.FromPaths(
                request.WorkspaceFolders?.Select(w => w.Uri.GetFileSystemPath()),
                request.RootPath);
            return Task.CompletedTask;
        }));

await server.WaitForExit;
