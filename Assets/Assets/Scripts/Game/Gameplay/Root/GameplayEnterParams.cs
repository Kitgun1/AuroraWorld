using System;
using AuroraWorld.GameRoot;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEnterParams : SceneEnterParams
    {
        public readonly string WorldSeed;

        public GameplayEnterParams(string worldSeed) : base(Scenes.GAMEPLAY)
        {
            WorldSeed = worldSeed ?? Guid.NewGuid().ToString();
        }
    }
}