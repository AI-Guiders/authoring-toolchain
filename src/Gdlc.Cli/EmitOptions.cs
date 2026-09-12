namespace Gdlc.Cli;

internal sealed class EmitOptions
{
    public required string Path { get; init; }

    public string Lang { get; init; } = "cs";

    public string? WorkspaceRoot { get; init; }

    public string Namespace { get; init; } = "Generated";

    public string ClassName { get; init; } = "Generated";

    public string? OutputPath { get; init; }

    public static bool TryParse(string[] args, out EmitOptions? options, out string? error)
    {
        options = null;
        error = null;

        var lang = "cs";
        string? path = null;
        string? workspaceRoot = null;
        var namespaceName = "Generated";
        var className = "Generated";
        string? outputPath = null;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("--lang=", StringComparison.Ordinal))
            {
                lang = arg["--lang=".Length..];
                continue;
            }

            switch (arg)
            {
                case "--lang" when i + 1 < args.Length:
                    lang = args[++i];
                    break;
                case "--workspace" or "-w" when i + 1 < args.Length:
                    workspaceRoot = args[++i];
                    break;
                case "--namespace" when i + 1 < args.Length:
                    namespaceName = args[++i];
                    break;
                case "--class" when i + 1 < args.Length:
                    className = args[++i];
                    break;
                case "--out" or "--output" when i + 1 < args.Length:
                    outputPath = args[++i];
                    break;
                default:
                    if (arg.StartsWith('-'))
                    {
                        error = $"emit: unknown argument `{arg}`";
                        return false;
                    }

                    if (path is not null)
                    {
                        error = "emit: multiple file paths";
                        return false;
                    }

                    path = arg;
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            error = "emit: missing file path";
            return false;
        }

        if (!string.Equals(lang, "cs", StringComparison.OrdinalIgnoreCase))
        {
            error = $"emit: unsupported --lang `{lang}` (wave-1 supports cs only)";
            return false;
        }

        options = new EmitOptions
        {
            Path = System.IO.Path.GetFullPath(path),
            Lang = lang,
            WorkspaceRoot = workspaceRoot,
            Namespace = namespaceName,
            ClassName = className,
            OutputPath = outputPath,
        };

        return true;
    }
}
