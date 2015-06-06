namespace Gitter.Services.Abstract
{
    public interface IApplicationStorageService
    {
        object Retrieve(string key);
        void Save(string key, object value);
        bool Exists(string key);
        void Remove(string key);
    }
}
