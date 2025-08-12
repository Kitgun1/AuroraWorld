using ObservableCollections;
using R3;

namespace AuroraWorld.Gameplay.GameColony
{
    public class ColonyTasksProxy
    {
        public ObservableList<MetaTaskProxy> MetaTasks;
        
        public readonly ColonyTasks Origin;

        public ColonyTasksProxy(ColonyTasks origin)
        {
            Origin = origin;
            MetaTasks = new ObservableList<MetaTaskProxy>();
            
            MetaTasks.ObserveAdd().Subscribe(t => Origin.MetaTasks.Add(t.Value.Origin));
            MetaTasks.ObserveRemove().Subscribe(t => Origin.MetaTasks.Remove(t.Value.Origin));
        }
    }
}