using Assets.Scripts.Services;
using AuroraWorld.App.Database;
using DI;

namespace AuroraWorld.GameRoot
{
    public static class GameRegistrations
    {
        public static void Register(DIContainer container)
        {
            container.RegisterFactory(_ => new Storage()).AsSingle();
            container.RegisterFactory(_ => new AnalyticsService()).AsSingle();
        }
    }
}