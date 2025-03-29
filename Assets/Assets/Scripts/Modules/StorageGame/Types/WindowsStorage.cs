using System.IO;
using Newtonsoft.Json;

namespace AuroraWorld.StorageGame
{
    public class WindowsStorage : IStorageType
    {
        private const string FOLDER = "storages/";

        public void Save<T>(string tag, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText($"{FOLDER}{tag}.json", json);
        }

        public T Load<T>(string tag, T defaultObj = default)
        {
            if (!File.Exists($"{FOLDER}{tag}.json")) return defaultObj;

            var json = File.ReadAllText($"{FOLDER}{tag}.json");
            return JsonConvert.DeserializeObject<T>(json) ?? defaultObj;
        }
    }
}