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

        private readonly IGitterApiService _gitterApiService;
        private readonly IPasswordStorageService _passwordStorageService;

        #endregion


        #region Constructor

        public SessionService(IGitterApiService gitterApiService, IPasswordStorageService passwordStorageService)
        {
            _gitterApiService = gitterApiService;
            _passwordStorageService = passwordStorageService;
        }

        #endregion


        #region Public Authentication Methods

        public async Task<bool?> LoginAsync()
        {
            return null;
            //return await _gitterApiService.LoginAsync(Credentials.OauthKey, Credentials.OauthSecret);
        }

        public void Logout()
        {
        }

#if WINDOWS_PHONE_APP
        public async Task<bool> FinalizeAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            return false;
            /*string token = await AuthenticationService.RetrieveTokenAsync(args);

            if (!string.IsNullOrWhiteSpace(token))
            {
                // Set token to use it next on API
                _gitterApiService.SetToken(token);

                // Save token in local storage
                _passwordStorageService.Save("token", token);

                return true;
            }

            return false;*/
        }
#endif

        #endregion
    }
}
