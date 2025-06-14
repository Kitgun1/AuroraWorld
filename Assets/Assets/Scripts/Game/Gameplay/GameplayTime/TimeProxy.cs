using System;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.GameplayTime
{
    public class TimeProxy
    {
        public ReactiveProperty<int> Tick;

        public Time Origin;

        private IDisposable _tickObservable;
        
        public TimeProxy(Time origin)
        {
            Origin = origin;

            Tick = new ReactiveProperty<int>();

            _tickObservable = Observable.Interval(TimeSpan.FromSeconds(0.1)).Subscribe(tick =>
            {
                Debug.Log($"tick: {tick}");
            }).AddTo(CreateRootObject());
        }

        private MonoBehaviour CreateRootObject()
        {
            var rootObject = new GameObject("[TIME]");
            return rootObject.GetComponent<MonoBehaviour>();
        }
    }
}