namespace AuroraWorld.Game.Lobby.Root
{
    public class LobbyEnterParams : SceneEnterParams
    {
        public string PreviousWorldName { get; }
        
        public LobbyEnterParams(string previousWorldName) : base(Scenes.LOBBY)
        {
            PreviousWorldName = previousWorldName;
        }
    }
}