using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.CommandPlane.Catalog.CodeGen;

namespace Authoring.Cli;

internal static class EmitCommand
{
    public static int Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("emit: missing file path");
            return 2;
        }

        var path = Path.GetFullPath(args[0]);
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"emit: file not found: {path}");
            return 1;
        }

        var namespaceName = "Generated.Catalog";
        var className = "PlanetCatalog";
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] is "--namespace" && i + 1 < args.Length)
            {
                namespaceName = args[++i];
            }
            else if (args[i] is "--class" && i + 1 < args.Length)
            {
                className = args[++i];
            }
        }

        var result = CatalogParser.ParseFile(path);
        if (result.Document is null)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.Error.WriteLine($"{diagnostic.Code}: {diagnostic.Message}");
            }

            return 1;
        }

        Console.WriteLine(CatalogCatalogEmitter.EmitCSharp(result.Document, namespaceName, className));
        return 0;
    }
}
