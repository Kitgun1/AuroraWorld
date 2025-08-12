using UnityEngine;

namespace AuroraWorld.Gameplay.GameColony
{
    public class ColonyTest : MonoBehaviour
    {
        private Colony _colony;

        private void Start()
        {
            _colony = new Colony(new Population(new Pawn("Васек", new PawnLocation(Vector3Int.zero))));
            
        }
    }
}