using R3;

namespace AuroraWorld.Gameplay.Player.Proxy
{
    public class SelectionSettingsProxy
    {
        public ReactiveProperty<int> Range { get; }
        public ReactiveProperty<bool> OnlyNeighbor { get; }
        public ReactiveProperty<bool> RemoveMode { get; }

        public SelectionSettings Origin;
        
        public SelectionSettingsProxy(SelectionSettings origin)
        {
            Origin = origin;
            
            Range = new ReactiveProperty<int>(origin.Range);
            OnlyNeighbor = new ReactiveProperty<bool>(origin.OnlyNeighbor);
            RemoveMode = new ReactiveProperty<bool>(origin.RemoveMode);

            Range.Skip(1).Subscribe(v => Origin.Range = v);
            OnlyNeighbor.Skip(1).Subscribe(v => Origin.OnlyNeighbor = v);
            RemoveMode.Skip(1).Subscribe(v => Origin.RemoveMode = v);
        }
    }
}