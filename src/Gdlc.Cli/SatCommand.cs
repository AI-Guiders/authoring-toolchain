using AIGuiders.Platform.Authoring.Emit;
using Gdlc.Plugins.Federation;

namespace Gdlc.Cli;

internal static class SatCommand
{
    public static int Run(string[] args)
    {
        GdlPluginBootstrap.Initialize();

        if (!GdlCommandOptions.TryParse(args, out var options, out var error))
        {
            Console.Error.WriteLine($"sat: {error}");
            return 2;
        }

        if (options!.ProjectPath is not null)
        {
            return SatProject(options);
        }

        if (!File.Exists(options.Path))
        {
            Console.Error.WriteLine($"sat: file not found: {options.Path}");
            return 1;
        }

        return SatSingleFile(options.Path!, options.Lang, options.Surface, options.WorkspaceRoot);
    }

    private static int SatProject(GdlCommandOptions options)
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
            var result = SatSingleFile(document.DisplayPath, options.Lang ?? load.DefaultLang, surface, load.Project.WorkspaceRoot, quiet: true);
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

    private static int SatSingleFile(string path, string lang, string? surface, string? workspaceRoot, bool quiet = false)
    {
        if (!GdlDocumentRunner.TryResolvePlugin(path, lang, surface, out var plugin, out var error))
        {
            Console.Error.WriteLine($"sat: {error}");
            return 2;
        }

        var result = plugin!.Sat(new GdlSatRequest
        {
            Path = path,
            Lang = lang,
            Surface = surface,
            WorkspaceRoot = workspaceRoot,
        });

        GdlDiagnosticsWriter.Write(result.Diagnostics);

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
