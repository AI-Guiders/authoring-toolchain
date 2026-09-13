using AIGuiders.Platform.Authoring.Emit;

namespace Gdlc.Cli;

internal static class GdlDocumentRunner
{
    public static int RunProject(
        GdlprojLoadResult load,
        string? surfaceOverride,
        Func<string, string?, int> runDocument)
    {
        if (load.Project is null)
        {
            GdlDiagnosticsWriter.Write(load.Diagnostics);
            return 1;
        }

        var exitCode = 0;
        foreach (var document in load.Project.Documents)
        {
            if (string.IsNullOrWhiteSpace(document.DisplayPath))
            {
                continue;
            }

            var surface = ResolveSurface(document.DisplayPath, surfaceOverride ?? load.DefaultSurface);
            var result = runDocument(document.DisplayPath, surface);
            if (result != 0)
            {
                exitCode = result;
            }
        }

        return exitCode;
    }

    public static string? ResolveSurface(string path, string? defaultSurface)
    {
        if (QuarryRouter.ResolveQuarryId(path) == GdlQuarryIds.Deck)
        {
            return string.IsNullOrWhiteSpace(defaultSurface) ? GdlSurfaceDefault.Wpf : defaultSurface;
        }

        return null;
    }

    public static bool TryResolvePlugin(
        string path,
        string lang,
        string? surface,
        out IGdlQuarryPlugin? plugin,
        out string? error)
    {
        var quarryId = QuarryRouter.ResolveQuarryId(path);
        if (quarryId is null)
        {
            plugin = null;
            error = QuarryRouter.DescribeUnsupported(path);
            return false;
        }

        var normalizedSurface = GdlQuarryRegistry.NormalizeSurface(quarryId, surface);
        return GdlQuarryRegistry.TryResolve(quarryId, lang, normalizedSurface, out plugin, out error);
    }
}
