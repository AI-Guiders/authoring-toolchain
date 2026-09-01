using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Authoring.LanguageServer.Catalog.Handlers;

internal static class CatalogStubDiagnostics
{
    public static IReadOnlyList<Diagnostic> Analyze(DocumentUri uri, string? text)
    {
        if (!string.Equals(Path.GetExtension(uri.GetFileSystemPath()), ".catalog", StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return
            [
                new Diagnostic
                {
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(0, 0), new Position(0, 0)),
                    Severity = DiagnosticSeverity.Warning,
                    Source = "authoring-catalog",
                    Message = "Empty .catalog file. Expected `catalog <planet>` header (see samples/catalog/).",
                },
            ];
        }

        if (!text.Contains("catalog ", StringComparison.Ordinal))
        {
            return
            [
                new Diagnostic
                {
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(new Position(0, 0), new Position(0, 1)),
                    Severity = DiagnosticSeverity.Error,
                    Source = "authoring-catalog",
                    Message = "Missing `catalog` header. Parser wiring pending AIGuiders.Platform.Authoring.Command.Catalog.",
                },
            ];
        }

        return [];
    }
}
