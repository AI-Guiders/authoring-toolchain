using Gdlc.Cli;
using Gdlc.Plugins.Federation;

GdlPluginBootstrap.Initialize();

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

return args[0] switch
{
    "emit" => EmitCommand.Run(args.Skip(1).ToArray()),
    "validate" => ValidateCommand.Run(args.Skip(1).ToArray()),
    "sat" => SatCommand.Run(args.Skip(1).ToArray()),
    "--version" or "-v" => PrintVersion(),
    "--help" or "-h" or "help" => PrintUsage(),
    _ => Unknown(args[0]),
};

static int PrintVersion()
{
    Console.WriteLine("gdlc 1.0.0");
    return 0;
}

static int PrintUsage()
{
    Console.WriteLine(
        """
        gdlc — unified GDL declare-time front-end (GUIDERS-ADR-0059 §10, ATC-ADR-0002)

        Plugin registry routes quarry + lang + surface to emit / validate / sat handlers.

        Usage:
          gdlc emit --lang=cs [--surface=wpf] <file.{quarry}.gdl> [--namespace N] [--class C] [--out path]
          gdlc emit --lang=cs [--surface=wpf] --project <file.gdlproj> [--out dir] [--namespace N]
          gdlc validate [--lang=cs] [--surface=wpf] <file.{quarry}.gdl> [--workspace <root>]
          gdlc validate [--lang=cs] [--surface=wpf] --project <file.gdlproj>
          gdlc sat [--lang=cs] [--surface=wpf] <file.{quarry}.gdl>
          gdlc sat [--lang=cs] [--surface=wpf] --project <file.gdlproj>
          gdlc --version
          gdlc --help

        Quarry routing (by filename suffix):
          *.catalog.gdl  → catalog / cs
          *.deck.gdl     → deck / cs / surface wpf (default)

        gdlproj directives:
          emit lang cs
          emit surface wpf

        Examples:
          gdlc emit --lang=cs samples/catalog/dash.catalog.gdl --namespace Dash.Generated --class DashCatalog
          gdlc emit --lang=cs --surface=wpf samples/deck/dashspec-studio.deck.gdl --namespace Dash.Generated --out Generated/DeckIds.g.cs
          gdlc validate --project samples/planet/planet.gdlproj
          gdlc sat --project samples/planet/planet.gdlproj
        """);

    return 0;
}

static int Unknown(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintUsage();
    return 2;
}
