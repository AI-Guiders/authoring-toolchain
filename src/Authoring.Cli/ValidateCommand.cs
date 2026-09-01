namespace Authoring.Cli;

using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.Authoring.Core;

internal static class ValidateCommand
{
    public static int Run(string[] args)
    {
        if (!CliCatalogWorkspace.TryParseCatalogArgs(args, out var path, out var workspaceRoot, out _))
        {
            Console.Error.WriteLine("validate: missing file path");
            return 2;
        }

        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"validate: file not found: {path}");
            return 1;
        }

        var result = CatalogCliSupport.OpenProject(path, workspaceRoot);
        CatalogCliSupport.WriteDiagnostics(result.Diagnostics);

        if (result.Document is null)
        {
            return 1;
        }

        if (CatalogCliSupport.HasFatalDiagnostics(result.Diagnostics))
        {
            return 1;
        }

        var importCount = result.Project?.Documents.Count(static d => d.Ref.Kind == AuthoringDocumentKind.FederationImport) ?? 0;
        Console.WriteLine(
            $"validate: ok — {CatalogSummary.Format(result.Document)} (project docs: {result.Project?.Documents.Count ?? 0}, wire imports: {importCount})");
        return 0;
    }
}

internal static class SummaryCommand
{
    public static int Run(string[] args)
    {
        if (!CliCatalogWorkspace.TryParseCatalogArgs(args, out var path, out var workspaceRoot, out _))
        {
            Console.Error.WriteLine("summary: missing file path");
            return 2;
        }

        var result = CatalogCliSupport.OpenProject(path, workspaceRoot);
        if (result.Document is null)
        {
            CatalogCliSupport.WriteDiagnostics(result.Diagnostics);
            return 1;
        }

        Console.WriteLine(CatalogSummary.Format(result.Document));
        return 0;
    }
}
