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
    }
}