using System.Linq;
using ObservableCollections;
using R3;

namespace Assets.Scripts.Game.Gameplay.World
{
    public class WorldStateProxy
    {
        public ObservableList<HexagonEntityProxy> Hexagons { get; } = new();

        public WorldStateProxy(WorldState worldState)
        {
            worldState.Hexagons.ForEach(hexagonEntity => Hexagons.Add(new HexagonEntityProxy(hexagonEntity)));

            Hexagons.ObserveAdd().Subscribe(e =>
            {
                var addedProxy = e.Value;
                worldState.Hexagons.Add(new HexagonEntity
                {
                    Position = addedProxy.Position,
                    BiomeType = addedProxy.BiomeType,
                });
            });

            Hexagons.ObserveRemove().Subscribe(e =>
            {
                var removedProxy = e.Value;
                var removedOriginal = worldState.Hexagons.FirstOrDefault(h => h.Position == removedProxy.Position);
                worldState.Hexagons.Remove(removedOriginal);
            });
        }
    }
}