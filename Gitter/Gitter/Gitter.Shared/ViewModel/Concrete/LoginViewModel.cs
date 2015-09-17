using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Gitter.API.Services.Abstract;
using Gitter.Services.Abstract;
using Gitter.ViewModel.Abstract;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Gitter.ViewModel.Concrete
{
    public sealed class LoginViewModel : ViewModelBase, ILoginViewModel
    {
        #region Services
        
        private readonly INavigationService _navigationService;
        private readonly ISessionService _sessionService;
        private readonly IPasswordStorageService _passwordStorageService;
        private readonly ILocalNotificationService _localNotificationService;

        #endregion


        #region Constructor

        public LoginViewModel(INavigationService navigationService,
            ISessionService sessionService,
            IPasswordStorageService passwordStorageService,
            ILocalNotificationService localNotificationService)
        {
            // Inject Services
            _navigationService = navigationService;
            _sessionService = sessionService;
            _passwordStorageService = passwordStorageService;
            _localNotificationService = localNotificationService;


            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }

        #endregion


        #region Methods

        public async Task LoginAsync()
        {
            bool isToShowMessage = false;

            try
            {
                // Retrieve token from local storage
                string token = _passwordStorageService.Retrieve("token");

                if (string.IsNullOrWhiteSpace(token))
                {
                    var auth = await _sessionService.LoginAsync();

#if !WINDOWS_PHONE_APP
                    if (auth == null)
                        isToShowMessage = true;

                    if (auth != null && auth.Value)
                        await FinalizeLoginAsync();
#endif
                }
                else
                {
                    await FinalizeLoginAsync();
                }
            }
            catch (Exception ex)
            {
                App.TelemetryClient.TrackException(ex);
                isToShowMessage = true;
            }

            if (isToShowMessage)
            {
                // Send error notification
                _localNotificationService.SendNotification("Authentication", "An error occured");
            }
        }

        private async Task FinalizeLoginAsync()
        {
            _navigationService.NavigateTo("Main");
        }


#if WINDOWS_PHONE_APP
        public async Task FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var result = await _sessionService.Finalize(args);
            if (result)
                await FinalizeLoginAsync();
        }
#endif

        #endregion
    }
}
