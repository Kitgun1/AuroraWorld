using AuroraWorld.Root;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEnterParams : SceneEnterParams
    {
        public string WorldName { get; set; }
        public int SeedHash { get; set; }

        public GameplayEnterParams(string worldName, int seedHash) : base(Scenes.GAMEPLAY)
        {
            WorldName = worldName;
            SeedHash = seedHash;
        }
    }
}