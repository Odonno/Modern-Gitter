using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Gitter.Services.Abstract;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class LoginViewModel : ViewModelBase, ILoginViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionService _sessionService;


        public LoginViewModel(INavigationService navigationService,
            ISessionService sessionService)
        {
            _navigationService = navigationService;
            _sessionService = sessionService;

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }


        public async Task LoginAsync()
        {
            bool isToShowMessage = false;

            try
            {
                var auth = await _sessionService.LoginAsync();

#if WINDOWS_APP
                if (auth == null)
                    isToShowMessage = true;

                if (auth != null && auth.Value)
                    await FinalizeLoginAsync();
#endif
            }
            catch (Exception ex)
            {
                App.TelemetryClient.TrackException(ex);
                isToShowMessage = true;
            }

            if (isToShowMessage)
            {
                // TODO : Send error notification
            }
        }

        private async Task FinalizeLoginAsync()
        {
            _navigationService.NavigateTo("Main");
        }


#if WINDOWS_PHONE_APP
        public async void Finalize(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var result = await _sessionService.Finalize(args);
            if (result)
                await FinalizeLoginAsync();
        }
#endif
    }
}
