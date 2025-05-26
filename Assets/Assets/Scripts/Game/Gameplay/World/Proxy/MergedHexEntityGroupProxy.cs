using ObservableCollections;
using R3;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class MergedHexEntityGroupProxy
    {
        public ObservableList<HexEntityProxy> HexagonEntities { get; } = new();
        public MergedHexEntityGroup Origin { get; }

        public MergedHexEntityGroupProxy(MergedHexEntityGroup group)
        {
            Origin = group;
            HexagonEntities.ObserveAdd().Subscribe(e =>
            {
                var addedProxy = e.Value;
                group.HexagonEntities.Add(addedProxy.Origin);
            });
            HexagonEntities.ObserveRemove().Subscribe(e =>
            {
                var removedProxy = e.Value;
                group.HexagonEntities.Remove(removedProxy.Origin);
            });
        }
    }
}