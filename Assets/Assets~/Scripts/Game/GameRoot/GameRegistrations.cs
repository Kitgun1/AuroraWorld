using Assets.Scripts.Services;
using AuroraWorld.App.Database;
using DI;

namespace AuroraWorld.GameRoot
{
    public static class GameRegistrations
    {
        public static void Register(DIContainer container)
        {
            container.RegisterInstance(new Storage());
            container.RegisterInstance(new AnalyticsService());
        }
    }
}