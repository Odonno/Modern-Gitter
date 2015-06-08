using Windows.Security.Credentials;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class PasswordStorageService : IPasswordStorageService
    {
        public void Save(string key, string password)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(key, "username", password));
        }

        public string Retrieve(string key)
        {
            try
            {
                var vault = new PasswordVault();
                return vault.Retrieve(key, "username").Password;
            }
            catch
            {
                return null;
            }
        }
    }
}
