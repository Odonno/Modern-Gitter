using Windows.Storage;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class ApplicationStorageService : IApplicationStorageService
    {
        public object Retrieve(string key)
        {
            return ApplicationData.Current.LocalSettings.Values[key];
        }

        public void Save(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
