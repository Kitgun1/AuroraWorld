namespace AuroraWorld.StorageGame
{
    public interface IStorageType
    {
        public void Save<T>(string tag, T obj);
        public T Load<T>(string tag, T defaultObj = default);
    }
}