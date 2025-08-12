using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraWorld.App.GameResources
{
    public class Resource<T> where T : Object
    {
        private readonly Dictionary<string, T> _loadedResources = new();

        /// <param name="path">Path to the asset in Resources folder (e.g., "Prefabs/Player")</param>
        public T Load(string path)
        {
            if (_loadedResources.TryGetValue(path, out var load)) return load;
            var resource = Resources.Load<T>(path);
            _loadedResources.Add(path, resource);
            return resource;
        }
    }
}