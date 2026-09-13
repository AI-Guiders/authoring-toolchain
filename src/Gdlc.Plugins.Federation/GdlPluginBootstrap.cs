using AIGuiders.Platform.Authoring.Emit;
using AIGuiders.Platform.Execution.CommandPlane.Catalog.CodeGen;
using AIGuiders.Surface.Wpf.CodeGen;

namespace Gdlc.Plugins.Federation;

public static class GdlPluginBootstrap
{
    public static void Initialize()
    {
        if (GdlQuarryRegistry.IsInitialized)
        {
            return;
        }

        GdlQuarryRegistry.Register(new CatalogQuarryPlugin());
        GdlQuarryRegistry.Register(new DeckQuarryPlugin());
        GdlQuarryRegistry.MarkInitialized();
    }
}
