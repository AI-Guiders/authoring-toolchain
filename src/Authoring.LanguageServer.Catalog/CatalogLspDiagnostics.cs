using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.Authoring.Core;

namespace Authoring.LanguageServer.Catalog;

using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

internal static class CatalogLspDiagnostics
{
    public static Container<Diagnostic> ToLsp(IReadOnlyList<AuthoringDiagnostic> diagnostics) =>
        new(diagnostics.Select(ToLsp).ToList());

    public static Container<Diagnostic> Analyze(string text)
    {
        var result = CatalogParser.Parse(text);
        return ToLsp(result.Diagnostics);
    }

    static Diagnostic ToLsp(AuthoringDiagnostic d) =>
        new()
        {
            Range = new Range(new Position(Math.Max(0, d.Line - 1), d.Column), new Position(Math.Max(0, d.Line - 1), d.Column + 1)),
            Severity = d.Code == AuthoringDiagnosticCode.NotationWireMismatch
                ? DiagnosticSeverity.Error
                : DiagnosticSeverity.Warning,
            Source = "authoring-catalog",
            Message = d.Message,
        };
}
