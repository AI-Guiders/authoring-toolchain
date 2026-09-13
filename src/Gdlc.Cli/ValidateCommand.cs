using AIGuiders.Platform.Authoring.Emit;
using Gdlc.Plugins.Federation;

namespace Gdlc.Cli;

internal static class ValidateCommand
{
    public static int Run(string[] args)
    {
        GdlPluginBootstrap.Initialize();

        if (!GdlCommandOptions.TryParse(args, out var options, out var error))
        {
            Console.Error.WriteLine($"validate: {error}");
            return 2;
        }

        if (options!.ProjectPath is not null)
        {
            return ValidateProject(options);
        }

        if (!File.Exists(options.Path))
        {
            Console.Error.WriteLine($"validate: file not found: {options.Path}");
            return 1;
        }

        return ValidateSingleFile(options.Path!, options.Lang, options.Surface, options.WorkspaceRoot);
    }

    private static int ValidateProject(GdlCommandOptions options)
    {
        if (!File.Exists(options.ProjectPath))
        {
            Console.Error.WriteLine($"validate: project not found: {options.ProjectPath}");
            return 1;
        }

        var load = GdlprojLoader.Open(options.ProjectPath);
        var exitCode = 0;
        var okCount = 0;

        var projectExit = GdlDocumentRunner.RunProject(
            load,
            options.Surface,
            (path, surface) =>
            {
                var result = ValidateSingleFile(path, options.Lang ?? load.DefaultLang, surface, load.Project!.WorkspaceRoot);
                if (result == 0)
                {
                    okCount++;
                }

                return result;
            });

        if (projectExit != 0)
        {
            exitCode = projectExit;
        }

        if (exitCode == 0)
        {
            Console.WriteLine($"validate: ok — {okCount} document(s) in `{Path.GetFileName(options.ProjectPath)}`");
        }

        return exitCode;
    }

    private static int ValidateSingleFile(string path, string lang, string? surface, string? workspaceRoot)
    {
        if (!GdlDocumentRunner.TryResolvePlugin(path, lang, surface, out var plugin, out var error))
        {
            Console.Error.WriteLine($"validate: {error}");
            return 2;
        }

        var result = plugin!.Validate(new GdlValidateRequest
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

        if (!string.IsNullOrWhiteSpace(result.Summary))
        {
            Console.WriteLine(result.Summary);
        }

        return 0;
    }
}
