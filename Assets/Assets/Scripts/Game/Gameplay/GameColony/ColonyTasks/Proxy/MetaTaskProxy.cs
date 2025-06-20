using ObservableCollections;
using R3;

namespace AuroraWorld.Gameplay.GameColony
{
    public class MetaTaskProxy
    {
        public readonly ObservableList<SubtaskProxy> Subtasks;
        public readonly int WorkerLimit;
        
        public readonly MetaTask Origin;

        public MetaTaskProxy(MetaTask origin)
        {
            Origin = origin;
            WorkerLimit = Origin.WorkerLimit;
            Subtasks = new ObservableList<SubtaskProxy>();

            Subtasks.ObserveAdd().Subscribe(t => Origin.Subtasks.Add(t.Value.Origin));
            Subtasks.ObserveRemove().Subscribe(t => Origin.Subtasks.Remove(t.Value.Origin));
        }
    }
}