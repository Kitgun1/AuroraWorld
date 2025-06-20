using AuroraWorld.GameRoot;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEnterParams : SceneEnterParams
    {
        public readonly string WorldName;

        public GameplayEnterParams(string worldName) : base(Scenes.GAMEPLAY)
        {
            WorldName = worldName;
        }
    }
}