using AIGuiders.Platform.Authoring.Emit;
using AIGuiders.Platform.Authoring.Sat;

namespace Gdlc.Plugins.Federation;

/// <summary>
/// Routes file-path sat requests through <see cref="IGdlQuarryPlugin.Sat"/> (ATC-ADR-0003).
/// </summary>
public sealed class QuarryPluginSatObserver : ISatObserver
{
    public string ObserverId => SatObserverIds.QuarryPlugin;

    public int Priority => 0;

    public bool CanObserve(SatContext context) =>
        !string.IsNullOrWhiteSpace(context.Path)
        && string.IsNullOrWhiteSpace(context.AdrId)
        && string.IsNullOrWhiteSpace(context.FactsPath);

    public SatRunResult Observe(SatContext context)
    {
        var quarryId = ResolveQuarryId(context.Path!);
        if (quarryId is null)
        {
            return SatRunResult.Skipped(DescribeUnsupported(context.Path!));
        }

        var surface = NormalizeSurface(quarryId, context.Surface);
        if (!GdlQuarryRegistry.TryResolve(quarryId, context.Lang, surface, out var plugin, out var error))
        {
            return SatRunResult.Failed(
                [new GdlDiagnostic("sat", error ?? "quarry plugin not found")]);
        }

        var gdlResult = plugin!.Sat(new GdlSatRequest
        {
            Path = context.Path!,
            Lang = context.Lang,
            Surface = surface,
            WorkspaceRoot = context.WorkspaceRoot,
        });

        return FromGdlSatResult(gdlResult);
    }

    private static SatRunResult FromGdlSatResult(GdlSatResult result)
    {
        if (!result.Success)
        {
            return SatRunResult.Failed(result.Diagnostics, result.Summary);
        }

        if (!result.Supported)
        {
            return SatRunResult.Skipped(result.Summary ?? "sat: skipped");
        }

        return SatRunResult.Ok(result.Summary ?? "sat: ok");
    }

    private static string? ResolveQuarryId(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.EndsWith(".catalog.gdl", StringComparison.OrdinalIgnoreCase))
        {
            return GdlQuarryIds.Catalog;
        }

        if (fileName.EndsWith(".deck.gdl", StringComparison.OrdinalIgnoreCase))
        {
            return GdlQuarryIds.Deck;
        }

        return null;
    }

    private static string? NormalizeSurface(string quarryId, string? surface) =>
        GdlQuarryRegistry.NormalizeSurface(quarryId, surface);

    private static string DescribeUnsupported(string path) =>
        $"sat: skipped — unsupported quarry for `{Path.GetFileName(path)}` (expected *.(catalog|deck).gdl)";
}
