namespace AuroraWorld.StorageGame
{
    public class YandexGameStorage : IStorageType
    {
        public void Save<T>(string tag, T obj)
        {
            throw new System.NotImplementedException();
        }

        public T Load<T>(string tag, T defaultObj = default)
        {
            throw new System.NotImplementedException();
        }
    }
}