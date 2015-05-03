using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Authentication.Web;
using Gitter.API.Configuration;
using Gitter.API.Services.Abstract;
using Gitter.Services.Abstract;
using Gitter.ViewModel;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Gitter.Services.Concrete
{
    public class SessionService : ISessionService
    {
        #region Services

        private readonly IGitterApiService _gitterApiService;

        #endregion


        #region Constructor

        public SessionService()
        {
            _gitterApiService = ViewModelLocator.GitterApi;
        }

        #endregion


        #region Public Authentication Methods

        public async Task<bool?> LoginAsync()
        {
            try
            {
                var startUri = new Uri(
                    string.Format("https://gitter.im/login/oauth/authorize?client_id={0}&response_type={1}&redirect_uri={2}",
                    Constants.OauthKey, 
                    "code",
                    Constants.RedirectUrl));
                var endUri = new Uri(Constants.RedirectUrl);

#if WINDOWS_PHONE_APP
                WebAuthenticationBroker.AuthenticateAndContinue(startUri, endUri, null, WebAuthenticationOptions.None);
                return null;
#else
                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
                return await GetSession(webAuthenticationResult);
#endif
            }
            catch
            {
                return null;
            }
        }

        public void Logout()
        {
        }


#if WINDOWS_PHONE_APP
        public async Task<bool> Finalize(WebAuthenticationBrokerContinuationEventArgs args)
        {
            try
            {
                return await GetSession(args.WebAuthenticationResult);
            }
            catch
            {
            }

            return false;
        }
#endif

        #endregion


        #region Private Authentication Methods

        private async Task<bool> GetSession(WebAuthenticationResult result)
        {
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var code = GetCode(result.ResponseData);
                var token = await GetToken(code);

                _gitterApiService.TryAuthenticate(token);

                return true;
            }
            if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                throw new Exception("Error http");
            }
            if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                throw new Exception("User Canceled");
            }

            return false;
        }

        private string GetCode(string webAuthResultResponseData)
        {
            var splitResultResponse = webAuthResultResponseData.Split('&');
            var codeString = splitResultResponse.FirstOrDefault(value => value.Contains("code"));
            var splitCode = codeString.Split('=');
            return splitCode.Last();
        }

        private async Task<string> GetToken(string code)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://gitter.im");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("client_id", Constants.OauthKey),
                    new KeyValuePair<string, string>("client_secret", Constants.OauthSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", Constants.RedirectUrl),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                var result = await httpClient.PostAsync("/login/oauth/token", content);
                string resultContent = result.Content.ReadAsStringAsync().Result;
                JsonObject value = JsonValue.Parse(resultContent).GetObject();

                return value.GetNamedString("access_token");
            }
        }

        #endregion
    }
}
