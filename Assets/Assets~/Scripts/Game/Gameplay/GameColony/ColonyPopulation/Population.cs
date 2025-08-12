using System.Collections.Generic;

namespace AuroraWorld.Gameplay.GameColony
{
    public class Population
    {
        public readonly List<Pawn> Pawns;
        // TODO: Add prisoners, guests

        public Population(params Pawn[] initialPawns)
        {
            Pawns = new List<Pawn>(initialPawns);
        }
    }
}