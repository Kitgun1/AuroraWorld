using ObservableCollections;
using R3;

namespace AuroraWorld.Gameplay.GameColony
{
    public class PopulationProxy
    {
        public readonly ObservableList<PawnProxy> Pawns;
        // TODO: Add prisoners, guests
        
        public readonly Population Origin;

        public PopulationProxy(Population origin)
        {
            Origin = origin;

            Pawns = new ObservableList<PawnProxy>();

            Origin.Pawns.ForEach(pawn => Pawns.Add(new PawnProxy(pawn)));

            Pawns.ObserveAdd().Subscribe(p => Origin.Pawns.Add(p.Value.Origin));
            Pawns.ObserveRemove().Subscribe(p => Origin.Pawns.Remove(p.Value.Origin));
        }
    }
}