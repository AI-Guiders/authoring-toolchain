using Gdlc.Cli;
using Xunit;

namespace Gdlc.Cli.SmokeTests;

public sealed class GdlcEmitSmokeTests
{
    [Fact]
    public void Emit_catalog_gdl_writes_csharp()
    {
        var sample = ResolveRepoPath("samples", "catalog", "dash.catalog.gdl");
        var exit = EmitCommand.Run(
        [
            "--lang=cs",
            sample,
            "--namespace",
            "Smoke.Generated",
            "--class",
            "SmokeCatalog",
        ]);

        Assert.Equal(0, exit);
    }

    [Fact]
    public void Emit_deck_gdl_writes_csharp()
    {
        var sample = ResolveRepoPath("samples", "deck", "dashspec-studio.deck.gdl");
        var exit = EmitCommand.Run(
        [
            "--lang=cs",
            sample,
            "--namespace",
            "Smoke.Generated",
            "--class",
            "DeckIds",
        ]);

        Assert.Equal(0, exit);
    }

    [Fact]
    public void Emit_unsupported_quarry_fails()
    {
        var path = Path.Combine(Path.GetTempPath(), $"gdlc-smoke-{Guid.NewGuid():N}.unknown.gdl");
        File.WriteAllText(path, "# stub");

        var exit = EmitCommand.Run(["--lang=cs", path]);

        Assert.Equal(2, exit);
    }

    private static string ResolveRepoPath(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine([dir.FullName, ..parts]);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException($"Could not resolve repo path: {string.Join('/', parts)}");
    }
}
