using AIGuiders.Platform.Authoring.Emit;
using Gdlc.Plugins.Federation;

namespace Gdlc.Cli;

internal static class EmitCommand
{
    public static int Run(string[] args)
    {
        GdlPluginBootstrap.Initialize();

        if (!EmitOptions.TryParse(args, out var options, out var error))
        {
            Console.Error.WriteLine($"emit: {error}");
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

        return EmitSingleFile(options.Path!, options.Lang, options.Surface, options.WorkspaceRoot, options.Namespace, options.ClassName, options.OutputPath);
    }

    private static int EmitProject(EmitOptions options)
    {
        if (!File.Exists(options.ProjectPath))
        {
            Console.Error.WriteLine($"emit: project not found: {options.ProjectPath}");
            return 1;
        }

        var load = GdlprojLoader.Open(options.ProjectPath);
        return GdlDocumentRunner.RunProject(
            load,
            options.Surface,
            (path, surface) =>
            {
                var documentOptions = new EmitOptions
                {
                    Path = path,
                    Lang = options.Lang ?? load.DefaultLang,
                    Surface = surface,
                    WorkspaceRoot = load.Project!.WorkspaceRoot,
                    Namespace = options.Namespace,
                    ClassName = EmitOutputNaming.DefaultClassName(path),
                    OutputPath = EmitOutputNaming.ResolveProjectOutputPath(
                        load.Project.WorkspaceRoot,
                        options.OutputPath,
                        path),
                };

                return EmitSingleFile(
                    documentOptions.Path!,
                    documentOptions.Lang,
                    documentOptions.Surface,
                    documentOptions.WorkspaceRoot,
                    documentOptions.Namespace,
                    documentOptions.ClassName,
                    documentOptions.OutputPath);
            });
    }

    private static int EmitSingleFile(
        string path,
        string lang,
        string? surface,
        string? workspaceRoot,
        string namespaceName,
        string className,
        string? outputPath)
    {
        if (!GdlDocumentRunner.TryResolvePlugin(path, lang, surface, out var plugin, out var error))
        {
            Console.Error.WriteLine($"emit: {error}");
            return 2;
        }

        var result = plugin!.Emit(new GdlEmitRequest
        {
            Path = path,
            Lang = lang,
            Surface = surface,
            WorkspaceRoot = workspaceRoot,
            Namespace = namespaceName,
            ClassName = className,
            OutputPath = outputPath,
        });

        if (!result.Success)
        {
            GdlDiagnosticsWriter.Write(result.Diagnostics);
            return 1;
        }

        return WriteOutput(result.GeneratedCode!, outputPath);
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
}
