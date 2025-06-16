using R3;

namespace AuroraWorld.Gameplay.Player.Proxy
{
    public class CameraSettingsProxy
    {
        public ReactiveProperty<float> MoveSpeed { get; }
        public ReactiveProperty<float> FastMoveSpeed { get; }

        public readonly CameraSettings Origin;

        public CameraSettingsProxy(CameraSettings origin)
        {
            Origin = origin;
            
            MoveSpeed = new ReactiveProperty<float>(origin.MoveSpeed);
            FastMoveSpeed = new ReactiveProperty<float>(origin.FastMoveSpeed);

            MoveSpeed.Skip(1).Subscribe(v => Origin.MoveSpeed = v);
            FastMoveSpeed.Skip(1).Subscribe(v => Origin.FastMoveSpeed = v);
        }
    }
}