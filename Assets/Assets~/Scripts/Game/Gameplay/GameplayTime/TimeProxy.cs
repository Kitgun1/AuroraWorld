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

        public int Ticks => Tick.Value + Hours * HOUR_IN_DAY;
        public int Hours => Hour.Value + Days * HOUR_IN_DAY;
        public int Days => Day.Value + Quarts * DAY_IN_QUART;
        public int Quarts => Quart.Value + Years * QUART_IN_YEAR;
        public int Years => Year.Value;

        private readonly Sun _sun;

        public const int TICK_IN_HOUR = 1200; // 1 час - 60 секунд
        public const int HOUR_IN_DAY = 24;
        public const int DAY_IN_QUART = 24;
        public const int QUART_IN_YEAR = 4;

        public TimeProxy(DIContainer container, Time origin)
        {
            container.RegisterInstance(this);
            Origin = origin;
            _sun = new Sun();

            Tick = new ReactiveProperty<int>(Origin.Tick);
            Hour = new ReactiveProperty<int>(Origin.Hour);
            Day = new ReactiveProperty<int>(Origin.Day);
            Quart = new ReactiveProperty<int>(Origin.Quart);
            Year = new ReactiveProperty<int>(Origin.Year);

            // 20 тиков в секунду
            Observable.Interval(TimeSpan.FromMilliseconds(50))
                .Subscribe(_ => Tick.Value = (Tick.Value + 1) % TICK_IN_HOUR)
                .AddTo(container.Resolve<Transform>("ParentMeshTransform"));

            Tick.Skip(1).Subscribe(t =>
            {
                Origin.Tick = t;
                if (t % TICK_IN_HOUR == 0)
                {
                    Hour.Value++;
                }

                if (Hour.Value != 0 && Hour.Value % HOUR_IN_DAY == 0)
                {
                    Hour.Value = 0;
                    Day.Value++;
                }

                if (Day.Value != 0 && Day.Value % DAY_IN_QUART == 0)
                {
                    Day.Value = 0;
                    Quart.Value++;
                }

                if (Quart.Value != 0 && Quart.Value % QUART_IN_YEAR == 0)
                {
                    Quart.Value = 0;
                    Year.Value++;
                }

                _sun.SetTick(t + Hour.Value * TICK_IN_HOUR);
            });

            Hour.Skip(1).Subscribe(h => Origin.Hour = h);
            Day.Skip(1).Subscribe(d => Origin.Day = d);
            Quart.Skip(1).Subscribe(q => Origin.Quart = q);
            Year.Skip(1).Subscribe(y => Origin.Year = y);
        }
    }
}