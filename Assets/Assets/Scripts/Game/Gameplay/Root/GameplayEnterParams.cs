using AuroraWorld.GameRoot;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEnterParams : SceneEnterParams
    {
        public readonly string WorldName;
        public readonly string WorldSeed;

        public GameplayEnterParams(string worldName, string worldSeed) : base(Scenes.GAMEPLAY)
        {
            WorldName = worldName;
            WorldSeed = worldSeed;
        }

    }
}