using System.IO;

namespace AuroraWorld.StorageGame
{
    public class Storage
    {
        private readonly IStorageType _storage;

        public Storage()
        {
#if UNITY_EDITOR
            _storage = new WindowsStorage();
            // TODO: storage for server, yandex game server
#endif
        }

        public void Save<T>(string tag, T obj) => _storage.Save(tag, obj);
        public T Load<T>(string tag, T defaultObj = default) => _storage.Load(tag, defaultObj);
    }
}