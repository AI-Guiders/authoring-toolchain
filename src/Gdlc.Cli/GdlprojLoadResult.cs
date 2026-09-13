using AIGuiders.Platform.Authoring.Core;

namespace Gdlc.Cli;

internal sealed class GdlprojLoadResult
{
    public AuthoringProject? Project { get; init; }

    public IReadOnlyList<AuthoringDiagnostic> Diagnostics { get; init; } = [];

    public string DefaultLang { get; init; } = "cs";

    public string? DefaultSurface { get; init; } = GdlSurfaceDefault.Wpf;
}
