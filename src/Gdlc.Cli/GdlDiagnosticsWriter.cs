using AIGuiders.Platform.Authoring.Core;
using AIGuiders.Platform.Authoring.Emit;

namespace Gdlc.Cli;

internal static class GdlDiagnosticsWriter
{
    public static void Write(IEnumerable<AuthoringDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.Error.WriteLine($"{diagnostic.Code}: {diagnostic.Message}");
        }
    }

    public static void Write(IEnumerable<GdlDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            var prefix = diagnostic.Line > 0 ? $"{diagnostic.Line}: " : string.Empty;
            Console.Error.WriteLine($"{diagnostic.Code}: {prefix}{diagnostic.Message}");
        }
    }
}
