using Gdlc.Cli;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

return args[0] switch
{
    "emit" => EmitCommand.Run(args.Skip(1).ToArray()),
    "--version" or "-v" => PrintVersion(),
    "--help" or "-h" or "help" => PrintUsage(),
    _ => Unknown(args[0]),
};

static int PrintVersion()
{
    Console.WriteLine("gdlc 0.1.0-wave1");
    return 0;
}

static int PrintUsage()
{
    Console.WriteLine(
        """
        gdlc — unified GDL declare-time front-end (wave-1 stub; GUIDERS-ADR-0059 §10)

        Today this routes single-file emit to existing per-quarry tools. Full pipeline
        (--project *.gdlproj, validate, sat) is not implemented yet.

        Usage:
          gdlc emit --lang=cs <file.{quarry}.gdl> [--namespace N] [--class C] [--out path]
          gdlc emit --lang=cs <file.catalog.gdl> [--workspace <root>]
          gdlc --version
          gdlc --help

        Quarry routing (by filename suffix):
          *.catalog.gdl  → catalog C# emit (authoring stack)
          *.deck.gdl     → deck C# emit (deck stack)

        Examples:
          gdlc emit --lang=cs samples/catalog/dash.catalog.gdl --namespace Dash.Generated --class DashCatalog
          gdlc emit --lang=cs samples/deck/dashspec-studio.deck.gdl --namespace Dash.Generated --out Generated/DeckIds.g.cs
        """);

    return 0;
}

static int Unknown(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintUsage();
    return 2;
}
