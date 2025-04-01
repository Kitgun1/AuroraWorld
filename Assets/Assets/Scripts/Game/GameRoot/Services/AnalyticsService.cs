using System;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class AnalyticsService : IDisposable
    {
        public AnalyticsService()
        {
            Debug.Log($"{nameof(AnalyticsService)} has been created!");
        }
        
        public void Dispose()
        {
            Debug.Log($"disposed: {nameof(AnalyticsService)}");
        }
    }
}