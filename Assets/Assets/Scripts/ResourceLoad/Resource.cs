using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraWorld.ResourceLoad
{
    public class Resource<T> where T : Object
    {
        private readonly Dictionary<string, T> _loadedResources = new();

        public T Load(string path)
        {
            if (_loadedResources.TryGetValue(path, out var load)) return load;
            var resource = Resources.Load<T>(path);
            _loadedResources.Add(path, resource);
            return resource;
        }
    }
}