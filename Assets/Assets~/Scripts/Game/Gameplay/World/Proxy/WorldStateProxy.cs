using DI;
using R3;

namespace AuroraWorld.Gameplay.World.Proxy
{
    public class WorldStateProxy
    {
        public readonly ReactiveProperty<string> WorldName;
        public readonly string WorldSeed;
        public readonly TerrainStateProxy TerrainState;
        public readonly EntityStateProxy EntityState;

        public readonly WorldState Origin;

        public WorldStateProxy(DIContainer container, WorldState origin)
        {
            container.RegisterInstance(this);
            Origin = origin;

            WorldName = new ReactiveProperty<string>(Origin.WorldName);
            WorldSeed = Origin.WorldSeed;
            EntityState = new EntityStateProxy(container, Origin.EntityState);
            TerrainState = new TerrainStateProxy(container, Origin.TerrainState, WorldSeed);
        }
    }
}