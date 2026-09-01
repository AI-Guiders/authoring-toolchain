using Authoring.Cli;

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

return args[0] switch
{
    "validate" => ValidateCommand.Run(args.Skip(1).ToArray()),
    "--version" or "-v" => PrintVersion(),
    "--help" or "-h" or "help" => PrintUsage(),
    _ => Unknown(args[0]),
};

static int PrintVersion()
{
    Console.WriteLine("authoring 0.1.0");
    return 0;
}

static int PrintUsage()
{
    Console.WriteLine(
        """
        authoring — federation authoring toolchain CLI (scaffold)

        Usage:
          authoring validate <file.catalog>   Stub validate (header check)
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
