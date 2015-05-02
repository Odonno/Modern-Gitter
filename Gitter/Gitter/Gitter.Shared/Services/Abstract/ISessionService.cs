using System.Threading.Tasks;

#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Gitter.Services.Abstract
{
    public interface ISessionService
    {
        Task<bool?> LoginAsync();
        void Logout();

#if WINDOWS_PHONE_APP
        Task<bool> Finalize(WebAuthenticationBrokerContinuationEventArgs args);
#endif
    }
}
