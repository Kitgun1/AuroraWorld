using UnityEngine;

namespace AuroraWorld.GameRoot
{
    [CreateAssetMenu(fileName = "new Curve", menuName = "Aurora World/Curve", order = 1)]
    public class Curve : ScriptableObject
    {
        public AnimationCurve AnimationCurve;
    }
}