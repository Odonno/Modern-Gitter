using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
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
        private readonly ITelemetryService _telemetryService;

        #endregion


        #region Constructor

        public LoginViewModel(INavigationService navigationService,
            ISessionService sessionService,
            IPasswordStorageService passwordStorageService,
            ILocalNotificationService localNotificationService,
            ITelemetryService telemetryService)
        {
            // Inject Services
            _navigationService = navigationService;
            _sessionService = sessionService;
            _passwordStorageService = passwordStorageService;
            _localNotificationService = localNotificationService;
            _telemetryService = telemetryService;


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


        #region Public Methods

        public async Task LoginAsync()
        {
            bool isToShowMessage = false;

            try
            {
                // Retrieve token from local storage
                string token = _passwordStorageService.Retrieve("token");

                if (string.IsNullOrWhiteSpace(token))
                {
                    bool? auth = await _sessionService.LoginAsync();

#if !WINDOWS_PHONE_APP
                    if (auth == null)
                        isToShowMessage = true;

                    if (auth != null && auth.Value)
                        LoginSuccess();
#endif
                }
                else
                {
                    LoginSuccess();
                }
            }
            catch (Exception ex)
            {
                _telemetryService.Client.TrackException(ex);
                isToShowMessage = true;
            }

            if (isToShowMessage)
            {
                // Send error notification
                _localNotificationService.SendNotification("Authentication", "An error occured");
            }
        }

#if WINDOWS_PHONE_APP
        public async Task FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            bool loginSuccess = await _sessionService.FinalizeAsync(args);
            if (loginSuccess)
                LoginSuccess();
        }
#endif

        #endregion


        #region Private Methods

        private void LoginSuccess()
        {
            _navigationService.NavigateTo("Main");
        }

        #endregion
    }
}
