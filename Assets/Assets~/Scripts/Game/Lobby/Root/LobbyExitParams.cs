using AuroraWorld.GameRoot;

namespace AuroraWorld.App.Lobby.Root
{
    public class LobbyExitParams
    {
        public SceneEnterParams TargetSceneEnterParams { get; }

        public LobbyExitParams(SceneEnterParams targetSceneEnterParams)
        {
            TargetSceneEnterParams = targetSceneEnterParams;
        }
    }
}