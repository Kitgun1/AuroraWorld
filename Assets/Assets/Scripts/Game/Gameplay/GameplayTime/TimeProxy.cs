using System;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.GameplayTime
{
    public class TimeProxy
    {
        public ReactiveProperty<int> Tick { get; }
        public ReactiveProperty<int> Hour { get; }
        public ReactiveProperty<int> Day { get; }
        public ReactiveProperty<int> Quart { get; }
        public ReactiveProperty<int> Year { get; }

        public readonly Time Origin;

        public TimeProxy(DIContainer container, Time origin)
        {
            container.RegisterInstance(this);
            Origin = origin;

            Tick = new ReactiveProperty<int>(Origin.Tick);
            Hour = new ReactiveProperty<int>(Origin.Hour);
            Day = new ReactiveProperty<int>(Origin.Day);
            Quart = new ReactiveProperty<int>(Origin.Quart);
            Year = new ReactiveProperty<int>(Origin.Year);

            Observable.Interval(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => Tick.Value = (Tick.Value + 1) % 10)
                .AddTo(container.Resolve<Transform>("ParentMeshTransform"));

            Tick.Skip(1).Subscribe(t =>
            {
                Origin.Tick = t;
                if (t % 10 == 0)
                {
                    Hour.Value++;
                }

                if (Hour.Value != 0 && Hour.Value % 24 == 0)
                {
                    Hour.Value = 0;
                    Day.Value++;
                }

                if (Day.Value != 0 && Day.Value % 24 == 0)
                {
                    Day.Value = 0;
                    Quart.Value++;
                }

                if (Quart.Value != 0 && Quart.Value % 4 == 0)
                {
                    Quart.Value = 0;
                    Year.Value++;
                }
            });

            Hour.Skip(1).Subscribe(h => Origin.Hour = h);
            Day.Skip(1).Subscribe(d => Origin.Day = d);
            Quart.Skip(1).Subscribe(q => Origin.Quart = q);
            Year.Skip(1).Subscribe(y => Origin.Year = y);
        }
    }
}