using Authoring.Cli;
using AIGuiders.Platform.Authoring.Command.Catalog;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

return args[0] switch
{
    "validate" => ValidateCommand.Run(args.Skip(1).ToArray()),
    "summary" => SummaryCommand.Run(args.Skip(1).ToArray()),
    "emit" => EmitCommand.Run(args.Skip(1).ToArray()),
    "--version" or "-v" => PrintVersion(),
    "--help" or "-h" or "help" => PrintUsage(),
    _ => Unknown(args[0]),
};

static int PrintVersion()
{
    Console.WriteLine("authoring 0.2.0");
    return 0;
}

static int PrintUsage()
{
    Console.WriteLine(
        """
        authoring — federation .catalog toolchain

        Usage:
          authoring validate <file.catalog>
          authoring summary <file.catalog>
          authoring emit <file.catalog> [--namespace N] [--class C]
          authoring --version
          authoring --help
        """);

    return 0;
}

static int Unknown(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintUsage();
    return 2;
}
