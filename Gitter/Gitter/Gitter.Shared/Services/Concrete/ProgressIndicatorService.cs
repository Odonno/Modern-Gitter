using System.Threading.Tasks;
using Gitter.Services.Abstract;
#if WINDOWS_PHONE_APP
using System;
using Windows.UI.ViewManagement;
#endif

namespace Gitter.Services.Concrete
{
    public class ProgressIndicatorService : IProgressIndicatorService
    {
#if WINDOWS_PHONE_APP
        public StatusBarProgressIndicator ProgressIndicator { get; set; }
#endif


        public async Task ShowAsync()
        {
#if WINDOWS_PHONE_APP
            if (ProgressIndicator != null)
                await ProgressIndicator.ShowAsync();
#endif
        }

        public async Task HideAsync()
        {
#if WINDOWS_PHONE_APP
            if (ProgressIndicator != null)
                await ProgressIndicator.HideAsync();
#endif
        }
    }
}
