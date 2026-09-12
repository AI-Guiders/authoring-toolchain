using AIGuiders.Platform.Authoring.Command.Bundles;
using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.Authoring.Core;
using AIGuiders.Platform.Authoring.Deck;
using AIGuiders.Platform.Execution.CommandPlane.Catalog.CodeGen;
using AIGuiders.Surface.Wpf.CodeGen;

namespace Gdlc.Cli;

internal static class EmitCommand
{
    public static int Run(string[] args)
    {
        if (!EmitOptions.TryParse(args, out var options, out var error))
        {
            Console.Error.WriteLine(error);
            return 2;
        }

        if (options!.ProjectPath is not null)
        {
            return EmitProject(options);
        }

        if (!File.Exists(options.Path))
        {
            Console.Error.WriteLine($"emit: file not found: {options.Path}");
            return 1;
        }

        return EmitSingleFile(options);
    }

    private static int EmitProject(EmitOptions options)
    {
        if (!File.Exists(options.ProjectPath))
        {
            Console.Error.WriteLine($"emit: project not found: {options.ProjectPath}");
            return 1;
        }

        var load = GdlprojLoader.Open(options.ProjectPath);
        if (load.Project is null)
        {
            WriteDiagnostics(load.Diagnostics);
            return 1;
        }

        var exitCode = 0;
        foreach (var document in load.Project.Documents)
        {
            if (string.IsNullOrWhiteSpace(document.DisplayPath))
            {
                continue;
            }

            var documentOptions = new EmitOptions
            {
                Path = document.DisplayPath,
                Lang = options.Lang,
                WorkspaceRoot = load.Project.WorkspaceRoot,
                Namespace = options.Namespace,
                ClassName = EmitOutputNaming.DefaultClassName(document.DisplayPath),
                OutputPath = EmitOutputNaming.ResolveProjectOutputPath(
                    load.Project.WorkspaceRoot,
                    options.OutputPath,
                    document.DisplayPath),
            };

            var result = EmitSingleFile(documentOptions);
            if (result != 0)
            {
                exitCode = result;
            }
        }

        return exitCode;
    }

    private static int EmitSingleFile(EmitOptions options)
    {
        return QuarryRouter.Resolve(options.Path!) switch
        {
            QuarryKind.Catalog => EmitCatalog(options),
            QuarryKind.Deck => EmitDeck(options),
            _ => UnsupportedQuarry(options.Path!),
        };
    }

    private static int UnsupportedQuarry(string path)
    {
        Console.Error.WriteLine(QuarryRouter.DescribeUnsupported(path));
        return 2;
    }

    private static int EmitCatalog(EmitOptions options)
    {
        var result = CatalogProject.Open(
            ResolveWorkspaceRoot(options.Path!, options.WorkspaceRoot),
            options.Path!,
            CatalogBundleLibrary.Federation);

        if (result.Document is null)
        {
            WriteDiagnostics(result.Diagnostics);
            return 1;
        }

        return WriteOutput(
            CatalogCatalogEmitter.EmitCSharp(result.Document, options.Namespace, options.ClassName),
            options.OutputPath);
    }

    private static int EmitDeck(EmitOptions options)
    {
        var result = DeckParser.ParseFile(options.Path!);
        if (result.Document is null)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.Error.WriteLine($"{options.Path}:{diagnostic.Line}: {diagnostic.Message}");
            }

            return 1;
        }

        return WriteOutput(
            DeckIdsEmitter.EmitCSharp(result.Document, options.Namespace, options.ClassName),
            options.OutputPath);
    }

    private static int WriteOutput(string code, string? outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            Console.WriteLine(code);
            return 0;
        }

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(outputPath, code);
        return 0;
    }

    private static string ResolveWorkspaceRoot(string catalogPath, string? explicitRoot)
    {
        if (!string.IsNullOrWhiteSpace(explicitRoot))
        {
            return Path.GetFullPath(explicitRoot);
        }

        var dir = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(catalogPath))!);
        while (dir is not null)
        {
            if (dir.GetFiles("*.slnx").Length > 0
                || dir.GetFiles("*.sln").Length > 0
                || Directory.Exists(Path.Combine(dir.FullName, ".git")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return Path.GetDirectoryName(Path.GetFullPath(catalogPath))!;
    }

    private static void WriteDiagnostics(IEnumerable<AuthoringDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.Error.WriteLine($"{diagnostic.Code}: {diagnostic.Message}");
        }
    }
}
