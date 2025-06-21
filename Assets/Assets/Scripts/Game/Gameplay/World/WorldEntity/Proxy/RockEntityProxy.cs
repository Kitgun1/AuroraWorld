using AuroraWorld.Gameplay.World.View;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class RockEntityProxy : WorldEntityProxy
    {
        public readonly RockEntity Origin;

        public RockEntityProxy(RockEntity origin) : base(origin)
        {
            Origin = origin;
            View = new WorldEntityView(Resources.Load<GameObject>("Prefabs/Entity/Rock"));
        }
    }
}