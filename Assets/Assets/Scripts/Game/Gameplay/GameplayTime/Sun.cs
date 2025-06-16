using System.Linq;
using AuroraWorld.App.GameResources;
using AuroraWorld.GameRoot;
using UnityEngine;
using LightType = UnityEngine.LightType;

namespace AuroraWorld.Gameplay.GameplayTime
{
    public class Sun
    {
        private readonly Light _sunObject;
        private readonly Quaternion _offsetQuaternion;
        private readonly Curve _curve;

        private const int TICK_IN_DAY = TimeProxy.HOUR_IN_DAY * TimeProxy.TICK_IN_HOUR;

        public Sun()
        {
            _sunObject = Object.FindObjectsOfType<Light>().First(l => l.type == LightType.Directional);
            _offsetQuaternion = _sunObject.transform.rotation;
            _curve = new Resource<Curve>().Load("Configurations/Curves/SunCurve");
        }

        public void SetTick(int tick)
        {
            var angle = Mathf.Lerp(0f, 360f, _curve.AnimationCurve.Evaluate((float)tick / TICK_IN_DAY));
            _sunObject.transform.rotation = Quaternion.Euler(angle, 0, 0) * _offsetQuaternion;
            
        }
    }
}