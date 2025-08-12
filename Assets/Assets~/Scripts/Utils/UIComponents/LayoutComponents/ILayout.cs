using R3;

namespace AuroraWorld.UIComponents.LayoutComponents
{
    public interface ILayout
    {
        bool StartOnAwake { get; set; }
        bool CalculateOnUpdate { get; set; }
        Subject<Unit> LayoutUpdate { get; }
        void CalculateLayout();
    }
}