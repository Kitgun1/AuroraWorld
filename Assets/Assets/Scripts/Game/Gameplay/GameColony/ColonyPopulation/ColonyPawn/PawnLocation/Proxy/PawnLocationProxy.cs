using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.GameColony
{
    public class PawnLocationProxy
    {
        public ReactiveProperty<Vector3Int> Position;

        public PawnLocation Origin;
        
        public PawnLocationProxy(PawnLocation origin)
        {
            Origin = origin;

            Position = new ReactiveProperty<Vector3Int>(Origin.Position);

            Position.Skip(1).Subscribe(v => Origin.Position = v);
        }
    }
}