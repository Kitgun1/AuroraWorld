using Assets.Scripts.Services;
using AuroraWorld.StorageGame;
using DI;

namespace AuroraWorld
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