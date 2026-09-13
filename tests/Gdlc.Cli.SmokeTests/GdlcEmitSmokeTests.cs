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

            "--surface=wpf",

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



    [Fact]

    public void Emit_project_gdlproj_writes_csharp()

    {

        var project = ResolveRepoPath("samples", "planet", "planet.gdlproj");

        var outputDir = Path.Combine(Path.GetTempPath(), $"gdlc-project-{Guid.NewGuid():N}");

        Directory.CreateDirectory(outputDir);



        var exit = EmitCommand.Run(

        [

            "--lang=cs",

            "--project",

            project,

            "--out",

            outputDir,

            "--namespace",

            "Smoke.Generated",

        ]);



        Assert.Equal(0, exit);

        Assert.True(File.Exists(Path.Combine(outputDir, "DashCatalog.g.cs")));

        Assert.True(File.Exists(Path.Combine(outputDir, "DashspecStudioDeckIds.g.cs")));

    }



    [Fact]
    public void Validate_deck_gdl_ok()
    {
        var sample = ResolveRepoPath("samples", "deck", "dashspec-studio.deck.gdl");
        var exit = ValidateCommand.Run(["--surface=wpf", sample]);
        Assert.Equal(0, exit);
    }

    [Fact]
    public void Validate_catalog_reports_diagnostics()
    {
        var sample = ResolveRepoPath("samples", "catalog", "dash.catalog.gdl");
        var exit = ValidateCommand.Run([sample, "--workspace", ResolveRepoRoot()]);
        Assert.Equal(1, exit);
    }

    [Fact]
    public void Validate_project_gdlproj_mixed()
    {
        var project = ResolveRepoPath("samples", "planet", "planet.gdlproj");
        var exit = ValidateCommand.Run(["--project", project]);
        Assert.Equal(1, exit);
    }



    [Fact]

    public void Sat_project_gdlproj_skips_without_config_quarry()

    {

        var project = ResolveRepoPath("samples", "planet", "planet.gdlproj");

        var exit = SatCommand.Run(["--project", project]);

        Assert.Equal(0, exit);

    }

    [Fact]
    public void Sat_adr_fsharp_0007_runs_observer()
    {
        var exit = SatCommand.Run(["--adr", "GUIDERS-FSHARP-ADR-0007"]);
        Assert.NotEqual(2, exit);
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



    private static string ResolveRepoRoot()

    {

        var sample = ResolveRepoPath("samples", "catalog", "dash.catalog.gdl");

        var dir = new DirectoryInfo(Path.GetDirectoryName(sample)!);

        while (dir is not null)

        {

            if (File.Exists(Path.Combine(dir.FullName, "AuthoringToolchain.slnx")))

            {

                return dir.FullName;

            }



            dir = dir.Parent;

        }



        throw new FileNotFoundException("Could not resolve authoring-toolchain root");

    }

}

