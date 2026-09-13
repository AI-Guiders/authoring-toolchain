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

    public static string? ResolveQuarryId(string path) =>
        Resolve(path) switch
        {
            QuarryKind.Catalog => AIGuiders.Platform.Authoring.Emit.GdlQuarryIds.Catalog,
            QuarryKind.Deck => AIGuiders.Platform.Authoring.Emit.GdlQuarryIds.Deck,
            _ => null,
        };

    public static string DescribeUnsupported(string path)
    {
        var fileName = Path.GetFileName(path);
        return $"unsupported quarry for `{fileName}` — expected *.(catalog|deck).gdl suffix";
    }
}
