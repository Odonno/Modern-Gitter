namespace Gitter.Services.Abstract
{
    public interface IPasswordStorageService
    {
        void Save(string key, string password);
        string Retrieve(string key);
    }
}
