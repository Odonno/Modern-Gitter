using System.Threading.Tasks;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Gitter.ViewModel.Abstract
{
    public interface ILoginViewModel
    {
        Task LoginAsync();

#if WINDOWS_PHONE_APP
        Task FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args);
#endif
    }
}
