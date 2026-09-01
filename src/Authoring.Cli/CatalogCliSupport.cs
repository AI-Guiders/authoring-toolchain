using AIGuiders.Platform.Authoring.Command.Bundles;
using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.Authoring.Core;

namespace Authoring.Cli;

internal static class CatalogCliSupport
{
    public static CatalogProjectResult OpenProject(string catalogPath, string? workspaceRoot) =>
        CatalogProject.Open(
            CliCatalogWorkspace.ResolveWorkspaceRoot(catalogPath, workspaceRoot),
            catalogPath,
            CatalogBundleLibrary.Federation);

    public static bool HasFatalDiagnostics(IReadOnlyList<AuthoringDiagnostic> diagnostics) =>
        diagnostics.Any(static d =>
            d.Code is AuthoringDiagnosticCode.GrammarWireMismatch
                or AuthoringDiagnosticCode.MissingCatalogHeader
                or AuthoringDiagnosticCode.MissingGrammarDeclaration
                or AuthoringDiagnosticCode.UnknownGrammarId
                or AuthoringDiagnosticCode.UnknownBundle
                or AuthoringDiagnosticCode.UnknownProfile
                or AuthoringDiagnosticCode.InvalidSyntax
                or AuthoringDiagnosticCode.EntryFileNotFound
                or AuthoringDiagnosticCode.EntryOutsideWorkspace);

    public static void WriteDiagnostics(IEnumerable<AuthoringDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            Console.Error.WriteLine($"{diagnostic.Code}: {diagnostic.Message}");
        }
    }
}
