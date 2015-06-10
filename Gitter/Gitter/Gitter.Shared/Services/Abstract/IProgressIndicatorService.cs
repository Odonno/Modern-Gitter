using System.Threading.Tasks;
using Windows.UI.ViewManagement;

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
