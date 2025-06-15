using AuroraWorld.Gameplay.Player.Proxy;

namespace AuroraWorld.Gameplay.Player
{
    public class UserSettings
    {
        public readonly SelectionSettingsProxy SelectionSettings;

        public UserSettings()
        {
            SelectionSettings = new SelectionSettingsProxy(new SelectionSettings()
            {
                Range = 3,
                OnlyNeighbor = true,
                RemoveMode = false
            });
        }
    }
}