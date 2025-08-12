using R3;
using UnityEngine;

namespace AuroraWorld.UIComponents.LayoutComponents
{
    public class ContentSizeFitterLayout : MonoBehaviour, ILayout
    {
        public RectTransform.Axis AxisFitter;
        public float PaddingEnd;
        public bool StartOnAwake { get; set; }
        public bool CalculateOnUpdate { get; set; }

        public Subject<Unit> LayoutUpdate { get; } = new Subject<Unit>();

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (StartOnAwake) CalculateLayout();
        }

        private void Update()
        {
            if (CalculateOnUpdate) CalculateLayout();
        }


        [ContextMenu(nameof(CalculateLayout))]
        public void CalculateLayout()
        {
#if UNITY_EDITOR
            _rectTransform = GetComponent<RectTransform>();
#endif
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out ILayout layout))
                {
                    layout.CalculateLayout();
                }
            }

            var lastChild = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
            var axisSize = AxisFitter == RectTransform.Axis.Horizontal
                ? PaddingEnd + lastChild.rect.xMax + lastChild.localPosition.x
                : PaddingEnd - lastChild.rect.yMin - lastChild.localPosition.y;
            _rectTransform.SetSizeWithCurrentAnchors(AxisFitter, axisSize);

            LayoutUpdate.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            LayoutUpdate.Dispose();
        }
    }
}