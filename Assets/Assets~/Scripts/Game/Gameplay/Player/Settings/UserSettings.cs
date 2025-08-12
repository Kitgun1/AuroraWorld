using AuroraWorld.Gameplay.Player.Proxy;

namespace AuroraWorld.Gameplay.Player
{
    public class UserSettings
    {
        public readonly SelectionSettingsProxy SelectionSettings;
        public readonly CameraSettingsProxy CameraSettings;

        public UserSettings()
        {
            SelectionSettings = new SelectionSettingsProxy(new SelectionSettings()
            {
                Range = 3,
                OnlyNeighbor = true,
                RemoveMode = false
            });

            CameraSettings = new CameraSettingsProxy(new CameraSettings()
            {
                MoveSpeed = 0.1f,
                FastMoveSpeed = 0.2f 
            });
        }
    }
}