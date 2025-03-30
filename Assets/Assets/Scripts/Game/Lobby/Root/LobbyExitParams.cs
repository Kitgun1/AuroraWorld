namespace AuroraWorld.Game.Lobby.Root
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