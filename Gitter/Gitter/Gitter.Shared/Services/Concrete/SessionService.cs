using System.Threading.Tasks;
using Gitter.Configuration;
using Gitter.Services.Abstract;
using GitterSharp.Services;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Gitter.Services.Concrete
{
    public class SessionService : ISessionService
    {
        #region Services

        private readonly AuthenticationService _authenticationService;
        private readonly IGitterApiService _gitterApiService;
        private readonly IPasswordStorageService _passwordStorageService;

        #endregion


        #region Constructor

        public SessionService(AuthenticationService authenticationService,
            IGitterApiService gitterApiService,
            IPasswordStorageService passwordStorageService)
        {
            _authenticationService = authenticationService;
            _gitterApiService = gitterApiService;
            _passwordStorageService = passwordStorageService;
        }

        #endregion


        #region Methods

        public async Task<bool?> LoginAsync()
        {
            bool? result = await _authenticationService.LoginAsync(Credentials.OauthKey, Credentials.OauthSecret);

#if WINDOWS_APP || WINDOWS_UWP
            string token = await _authenticationService.RetrieveTokenAsync();
            result = SetToken(token);
#endif

            return result;
        }

        public void Logout()
        {
        }

#if WINDOWS_PHONE_APP
        public async Task<bool> FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            string token = await _authenticationService.RetrieveTokenAsync(args);
            return SetToken(token);
        }
#endif

        private bool SetToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Set token to use it next on API
                _gitterApiService.SetToken(token);

                // Save token in local storage
                _passwordStorageService.Save("token", token);

                return true;
            }

            return false;
        }

        #endregion
    }
}
