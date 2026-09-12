namespace Gdlc.Cli;

internal enum QuarryKind
{
    Unsupported,
    Catalog,
    Deck,
}

internal static class QuarryRouter
{
    public static QuarryKind Resolve(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.EndsWith(".catalog.gdl", StringComparison.OrdinalIgnoreCase))
        {
            return QuarryKind.Catalog;
        }

        if (fileName.EndsWith(".deck.gdl", StringComparison.OrdinalIgnoreCase))
        {
            return QuarryKind.Deck;
        }

        return QuarryKind.Unsupported;
    }

    public static string DescribeUnsupported(string path)
    {
        var fileName = Path.GetFileName(path);
        return $"emit: unsupported quarry for `{fileName}` — expected *.(catalog|deck).gdl suffix";
    }
}
