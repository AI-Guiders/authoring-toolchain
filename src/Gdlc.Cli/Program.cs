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
    Console.WriteLine("gdlc 0.2.0-wave2");
    return 0;
}

static int PrintUsage()
{
    Console.WriteLine(
        """
        gdlc — unified GDL declare-time front-end (GUIDERS-ADR-0059 §10)

        Routes single-file or project emit to existing per-quarry stacks.
        validate / sat are not implemented yet.

        Usage:
          gdlc emit --lang=cs <file.{quarry}.gdl> [--namespace N] [--class C] [--out path]
          gdlc emit --lang=cs --project <file.gdlproj> [--out dir]
          gdlc emit --lang=cs <file.catalog.gdl> [--workspace <root>]
          gdlc --version
          gdlc --help

        Quarry routing (by filename suffix):
          *.catalog.gdl  → catalog C# emit (authoring stack)
          *.deck.gdl     → deck C# emit (deck stack)

        Examples:
          gdlc emit --lang=cs samples/catalog/dash.catalog.gdl --namespace Dash.Generated --class DashCatalog
          gdlc emit --lang=cs samples/deck/dashspec-studio.deck.gdl --namespace Dash.Generated --out Generated/DeckIds.g.cs
          gdlc emit --lang=cs --project samples/planet/planet.gdlproj --out Generated
        """);

    return 0;
}

static int Unknown(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintUsage();
    return 2;
}
