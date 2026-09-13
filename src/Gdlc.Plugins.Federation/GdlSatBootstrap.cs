using AIGuiders.Platform.Authoring.Sat;
using AIGuiders.Platform.Authoring.Sat.IdeSession;

namespace Gdlc.Plugins.Federation;

public static class GdlSatBootstrap
{
    public static void Initialize()
    {
        GdlPluginBootstrap.Initialize();

        if (SatObserverRegistry.IsInitialized)
        {
            return;
        }

        SatObserverRegistry.Register(new QuarryPluginSatObserver());
        SatObserverRegistry.Register(new AdrFactsSatObserver());
        SatObserverRegistry.Register(new ConfigSatObserver());
        SatObserverRegistry.Register(new IdeSessionSatObserver());
        SatObserverRegistry.MarkInitialized();
    }
}
