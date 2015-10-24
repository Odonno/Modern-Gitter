using System.Threading.Tasks;
#if WINDOWS_PHONE_APP
using Windows.UI.ViewManagement;
#endif

namespace Gitter.Services.Abstract
{
    public interface IProgressIndicatorService
    {
#if WINDOWS_PHONE_APP
        StatusBarProgressIndicator ProgressIndicator { get; set; }
#endif
        
        Task ShowAsync();
        Task HideAsync();
    }
}
