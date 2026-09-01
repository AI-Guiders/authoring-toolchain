using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Authoring.LanguageServer.Catalog;

internal static class CatalogLspHelpers
{
    public static readonly TextDocumentSelector DocumentSelector = new(
        new TextDocumentFilter { Language = "catalog" });
}
