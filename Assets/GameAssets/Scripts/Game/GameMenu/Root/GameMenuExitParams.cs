using AuroraWorld.Root;

namespace AuroraWorld.GameMenu.Root
{
    public class GameMenuExitParams
    {
        public SceneEnterParams TargetSceneEnterParams;

        public GameMenuExitParams(SceneEnterParams targetSceneEnterParams)
        {
            TargetSceneEnterParams = targetSceneEnterParams;
        }
    }
}