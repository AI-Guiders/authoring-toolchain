using AIGuiders.Platform.Authoring.Sat;
using Gdlc.Plugins.Federation;

namespace Gdlc.Cli;

internal static class SatCommand
{
    public static int Run(string[] args)
    {
        GdlSatBootstrap.Initialize();

        if (!SatOptions.TryParse(args, out var options, out var error))
        {
            Console.Error.WriteLine($"sat: {error}");
            return 2;
        }

        if (options!.UsesHoareObservers)
        {
            return SatHoareObservers(options);
        }

        if (options.ProjectPath is not null)
        {
            return SatProject(options);
        }

        if (!File.Exists(options.Path))
        {
            Console.Error.WriteLine($"sat: file not found: {options.Path}");
            return 1;
        }

        return SatSingleFile(options);
    }

    private static int SatHoareObservers(SatOptions options)
    {
        if (options.ProjectPath is not null)
        {
            return SatHoareProject(options);
        }

        if (!string.IsNullOrWhiteSpace(options.FactsPath) && !File.Exists(options.FactsPath))
        {
            Console.Error.WriteLine($"sat: facts file not found: {options.FactsPath}");
            return 1;
        }

        if (!string.IsNullOrWhiteSpace(options.Path) && !File.Exists(options.Path))
        {
            Console.Error.WriteLine($"sat: file not found: {options.Path}");
            return 1;
        }

        return ObserveContext(BuildContext(options), quiet: false);
    }

    private static int SatHoareProject(SatOptions options)
    {
        if (!File.Exists(options.ProjectPath))
        {
            Console.Error.WriteLine($"sat: project not found: {options.ProjectPath}");
            return 1;
        }

        var load = GdlprojLoader.Open(options.ProjectPath);
        if (load.Project is null)
        {
            GdlDiagnosticsWriter.Write(load.Diagnostics);
            return 1;
        }

        var supportedCount = 0;
        var skippedCount = 0;
        var exitCode = 0;

        foreach (var document in load.Project.Documents)
        {
            if (string.IsNullOrWhiteSpace(document.DisplayPath))
            {
                continue;
            }

            var surface = GdlDocumentRunner.ResolveSurface(
                document.DisplayPath,
                options.Surface ?? load.DefaultSurface);
            var context = BuildContext(options, document.DisplayPath, surface, load.Project.WorkspaceRoot);
            var result = ObserveContext(context, quiet: true);
            if (result == 0)
            {
                supportedCount++;
            }
            else if (result == 3)
            {
                skippedCount++;
            }
            else
            {
                exitCode = result;
            }
        }

        if (exitCode != 0)
        {
            return exitCode;
        }

        if (supportedCount == 0)
        {
            Console.WriteLine($"sat: ok — no supported observers for {skippedCount} document(s) in `{Path.GetFileName(options.ProjectPath)}`");
            return 0;
        }

        Console.WriteLine($"sat: ok — {supportedCount} observer(s), {skippedCount} skipped in `{Path.GetFileName(options.ProjectPath)}`");
        return 0;
    }

    private static int SatProject(SatOptions options)
    {
        if (!File.Exists(options.ProjectPath))
        {
            Console.Error.WriteLine($"sat: project not found: {options.ProjectPath}");
            return 1;
        }

        var load = GdlprojLoader.Open(options.ProjectPath);
        if (load.Project is null)
        {
            GdlDiagnosticsWriter.Write(load.Diagnostics);
            return 1;
        }

        var supportedCount = 0;
        var skippedCount = 0;
        var exitCode = 0;

        foreach (var document in load.Project.Documents)
        {
            if (string.IsNullOrWhiteSpace(document.DisplayPath))
            {
                continue;
            }

            var surface = GdlDocumentRunner.ResolveSurface(
                document.DisplayPath,
                options.Surface ?? load.DefaultSurface);
            var context = BuildContext(options, document.DisplayPath, surface, load.Project.WorkspaceRoot);
            var result = ObserveContext(context, quiet: true);
            if (result == 0)
            {
                supportedCount++;
            }
            else if (result == 3)
            {
                skippedCount++;
            }
            else
            {
                exitCode = result;
            }
        }

        if (exitCode != 0)
        {
            return exitCode;
        }

        if (supportedCount == 0)
        {
            Console.WriteLine($"sat: ok — no sat observers for {skippedCount} document(s); config quarry required (ADR-0064)");
            return 0;
        }

        Console.WriteLine($"sat: ok — {supportedCount} observer(s), {skippedCount} skipped");
        return 0;
    }

    private static int SatSingleFile(SatOptions options) =>
        ObserveContext(BuildContext(options), quiet: false);

    private static SatContext BuildContext(
        SatOptions options,
        string? path = null,
        string? surface = null,
        string? workspaceRoot = null) =>
        new()
        {
            Path = path ?? options.Path,
            ProjectPath = options.ProjectPath,
            WorkspaceRoot = workspaceRoot ?? options.WorkspaceRoot,
            Lang = options.Lang,
            Surface = surface ?? options.Surface,
            AdrId = options.AdrId,
            FactsPath = options.FactsPath,
        };

    private static int ObserveContext(SatContext context, bool quiet)
    {
        if (!SatObserverRegistry.TryObserve(context, out var result, out var error))
        {
            Console.Error.WriteLine($"sat: {error}");
            return 2;
        }

        return HandleSatRunResult(result!, quiet);
    }

    private static int HandleSatRunResult(SatRunResult result, bool quiet)
    {
        if (result.Diagnostics.Count > 0)
        {
            GdlDiagnosticsWriter.Write(result.Diagnostics);
        }

        if (!result.Success)
        {
            return 1;
        }

        if (!result.Supported)
        {
            if (!quiet && !string.IsNullOrWhiteSpace(result.Summary))
            {
                Console.WriteLine(result.Summary);
            }

            return 3;
        }

        if (!quiet && !string.IsNullOrWhiteSpace(result.Summary))
        {
            Console.WriteLine(result.Summary);
        }

        return 0;
    }
}
