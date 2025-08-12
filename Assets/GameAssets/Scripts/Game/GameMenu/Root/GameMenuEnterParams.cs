using AuroraWorld.Root;

namespace AuroraWorld.GameMenu.Root
{
    public class GameMenuEnterParams : SceneEnterParams
    {
        public bool IsFirstRun { get; }

        public GameMenuEnterParams(bool isFirstRun) : base(Scenes.STARTING_MENU)
        {
            IsFirstRun = isFirstRun;
        }
    }
}