using AuroraWorld.GameRoot;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayExitParams
    {
        public SceneEnterParams TargetSceneEnterParams { get; }

        public GameplayExitParams(SceneEnterParams targetSceneEnterParams)
        {
            TargetSceneEnterParams = targetSceneEnterParams;
        }
    }
}