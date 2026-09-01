using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.CommandPlane.Catalog.CodeGen;

namespace Authoring.Cli;

internal static class EmitCommand
{
    public static int Run(string[] args)
    {
        if (!CliCatalogWorkspace.TryParseCatalogArgs(args, out var path, out var workspaceRoot, out var passthrough))
        {
            Console.Error.WriteLine("emit: missing file path");
            return 2;
        }

        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"emit: file not found: {path}");
            return 1;
        }

        var namespaceName = "Generated.Catalog";
        var className = "PlanetCatalog";
        for (var i = 1; i < passthrough.Length; i++)
        {
            if (passthrough[i] is "--namespace" && i + 1 < passthrough.Length)
            {
                namespaceName = passthrough[++i];
            }
            else if (passthrough[i] is "--class" && i + 1 < passthrough.Length)
            {
                className = passthrough[++i];
            }
        }

        var result = CatalogCliSupport.OpenProject(path, workspaceRoot);
        if (result.Document is null)
        {
            CatalogCliSupport.WriteDiagnostics(result.Diagnostics);
            return 1;
        }

        Console.WriteLine(CatalogCatalogEmitter.EmitCSharp(result.Document, namespaceName, className));
        return 0;
    }
}
