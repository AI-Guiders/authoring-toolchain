using AIGuiders.Platform.Authoring.Core;
using AIGuiders.Platform.Modeling.Paths;

namespace Gdlc.Cli;

/// <summary>
/// Minimal <c>*.gdlproj</c> loader → <see cref="AuthoringProject"/> (GUIDERS-ADR-0051 / 0059).
/// </summary>
internal static class GdlprojLoader
{
    public static AuthoringProjectLoadResult Open(string gdlprojPath)
    {
        var diagnostics = new List<AuthoringDiagnostic>();
        var physical = Path.GetFullPath(gdlprojPath);

        if (!File.Exists(physical))
        {
            diagnostics.Add(new(
                AuthoringDiagnosticCode.EntryFileNotFound,
                $"Project file not found: `{physical}`.",
                1));
            return new() { Diagnostics = diagnostics };
        }

        if (!physical.EndsWith(".gdlproj", StringComparison.OrdinalIgnoreCase))
        {
            diagnostics.Add(new(
                AuthoringDiagnosticCode.InvalidSyntax,
                $"Not a gdlproj file: `{physical}`.",
                1));
            return new() { Diagnostics = diagnostics };
        }

        var projectDirectory = Path.GetDirectoryName(physical)!;
        var documentPaths = new List<string>();
        var lineNo = 0;

        foreach (var rawLine in File.ReadLines(physical))
        {
            lineNo++;
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                diagnostics.Add(new(
                    AuthoringDiagnosticCode.InvalidSyntax,
                    $"gdlproj:{lineNo}: invalid line `{rawLine}`",
                    lineNo));
                continue;
            }

            switch (parts[0].ToLowerInvariant())
            {
                case "project":
                    break;
                case "document":
                    documentPaths.Add(parts[1]);
                    break;
                default:
                    diagnostics.Add(new(
                        AuthoringDiagnosticCode.InvalidSyntax,
                        $"gdlproj:{lineNo}: unknown directive `{parts[0]}`",
                        lineNo));
                    break;
            }
        }

        if (documentPaths.Count == 0)
        {
            diagnostics.Add(new(
                AuthoringDiagnosticCode.InvalidSyntax,
                "gdlproj: at least one `document` entry is required.",
                1));
            return new() { Diagnostics = diagnostics };
        }

        if (diagnostics.Exists(d => d.Code == AuthoringDiagnosticCode.InvalidSyntax))
        {
            return new() { Diagnostics = diagnostics };
        }

        var resolvedDocuments = new List<(string Physical, string Relative)>();
        foreach (var relativePath in documentPaths)
        {
            var docPhysical = Path.GetFullPath(Path.Combine(projectDirectory, relativePath));
            if (!File.Exists(docPhysical))
            {
                diagnostics.Add(new(
                    AuthoringDiagnosticCode.EntryFileNotFound,
                    $"Document not found: `{docPhysical}`.",
                    1));
                continue;
            }

            resolvedDocuments.Add((docPhysical, relativePath));
        }

        if (resolvedDocuments.Count == 0)
        {
            return new() { Diagnostics = diagnostics };
        }

        var workspaceRoot = ResolveWorkspaceRoot(projectDirectory, resolvedDocuments.Select(d => d.Physical));
        var documents = new List<ResolvedAuthoringDocument>();
        LogicalPath? entry = null;

        foreach (var (docPhysical, _) in resolvedDocuments)
        {
            var logical = PathBoundary.ToLogical(workspaceRoot, docPhysical);
            if (logical is null)
            {
                diagnostics.Add(new(
                    AuthoringDiagnosticCode.EntryOutsideWorkspace,
                    $"Document `{docPhysical}` is outside workspace `{workspaceRoot}`.",
                    1));
                continue;
            }

            entry ??= logical;
            var text = File.ReadAllText(docPhysical);
            documents.Add(ResolvedAuthoringDocument.LogicalFile(logical.Value, text, docPhysical));
        }

        if (documents.Count == 0 || entry is null)
        {
            return new() { Diagnostics = diagnostics };
        }

        var project = new AuthoringProject(workspaceRoot, entry.Value, documents);
        return new() { Project = project, Diagnostics = diagnostics };
    }

    private static string ResolveWorkspaceRoot(string projectDirectory, IEnumerable<string> documentPaths)
    {
        var roots = documentPaths.Prepend(projectDirectory).Select(Path.GetFullPath).ToArray();
        var common = roots[0];

        foreach (var path in roots.Skip(1))
        {
            common = GetCommonPath(common, path);
        }

        return common;
    }

    private static string GetCommonPath(string left, string right)
    {
        var leftParts = Path.GetFullPath(left).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var rightParts = Path.GetFullPath(right).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var length = Math.Min(leftParts.Length, rightParts.Length);
        var index = 0;

        while (index < length
               && string.Equals(leftParts[index], rightParts[index], StringComparison.OrdinalIgnoreCase))
        {
            index++;
        }

        if (index == 0)
        {
            return Path.GetPathRoot(left) ?? left;
        }

        return string.Join(Path.DirectorySeparatorChar, leftParts[..index]);
    }
}
