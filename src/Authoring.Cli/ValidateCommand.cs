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

        var text = File.ReadAllText(path);
        if (!text.Contains("catalog ", StringComparison.Ordinal))
        {
            Console.Error.WriteLine("validate: missing `catalog` header");
            return 1;
        }

        Console.WriteLine($"validate: ok (stub) — {path}");
        return 0;
    }
}
