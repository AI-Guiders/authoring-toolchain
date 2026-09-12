using System.Diagnostics;
using Xunit;

namespace GdlEmit.MsBuildTests;

public sealed class GdlEmitProjectTargetsTests
{
    [Fact]
    public void GdlEmitProject_sample_builds_and_verify_passes()
    {
        var repoRoot = ResolveRepoRoot();
        var sample = Path.Combine(repoRoot, "samples", "GdlEmit.Project.Sample", "GdlEmit.Project.Sample.csproj");
        Assert.True(File.Exists(sample), $"Sample project not found: {sample}");

        RunProcess("dotnet", $"build \"{sample}\" -c Release", repoRoot);
        RunProcess("dotnet", $"msbuild \"{sample}\" -p:Configuration=Release -t:GdlEmitVerify", repoRoot);
    }

    private static void RunProcess(string fileName, string arguments, string workingDirectory)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        });

        Assert.NotNull(process);
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Assert.True(
            process.ExitCode == 0,
            $"{fileName} {arguments} failed ({process.ExitCode}).\nstdout:\n{stdout}\nstderr:\n{stderr}");
    }

    private static string ResolveRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AuthoringToolchain.slnx")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not resolve authoring-toolchain repo root.");
    }
}
