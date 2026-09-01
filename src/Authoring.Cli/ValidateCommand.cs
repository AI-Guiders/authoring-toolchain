using AIGuiders.Platform.Authoring.Command.Bundles;
using AIGuiders.Platform.Authoring.Command.Catalog;
using AIGuiders.Platform.Authoring.Core;

namespace Authoring.Cli;

internal static class ValidateCommand
{
    public static int Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("validate: missing file path");
            return 2;
        }

        var path = Path.GetFullPath(args[0]);
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"validate: file not found: {path}");
            return 1;
        }

        var result = CatalogParser.ParseFile(path, CatalogBundleLibrary.Federation);
        foreach (var diagnostic in result.Diagnostics)
        {
            Console.Error.WriteLine($"{diagnostic.Code}: {diagnostic.Message}");
        }

        if (result.Document is null)
        {
            return 1;
        }

        var fatal = result.Diagnostics.Any(static d =>
            d.Code is AuthoringDiagnosticCode.GrammarWireMismatch
                or AuthoringDiagnosticCode.MissingCatalogHeader
                or AuthoringDiagnosticCode.MissingGrammarDeclaration
                or AuthoringDiagnosticCode.UnknownGrammarId
                or AuthoringDiagnosticCode.UnknownBundle
                or AuthoringDiagnosticCode.UnknownProfile
                or AuthoringDiagnosticCode.InvalidSyntax);

        if (fatal)
        {
            return 1;
        }

        Console.WriteLine($"validate: ok — {CatalogSummary.Format(result.Document)}");
        return 0;
    }
}

internal static class SummaryCommand
{
    public static int Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("summary: missing file path");
            return 2;
        }

        var path = Path.GetFullPath(args[0]);
        var result = CatalogParser.ParseFile(path, CatalogBundleLibrary.Federation);
        if (result.Document is null)
        {
            return 1;
        }

        Console.WriteLine(CatalogSummary.Format(result.Document));
        return 0;
    }
}
