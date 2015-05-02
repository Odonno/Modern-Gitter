using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using Gitter.API.Services.Abstract;
using Gitter.Model;
using Newtonsoft.Json;

namespace Gitter.API.Services.Concrete
{
    public class GitterApiService : IGitterApiService
    {
        #region Fields

        private const string BaseUrl = "https://api.gitter.im/";
        private const string Version = "v1/";

        private HttpClient HttpClient
        {
            get
            {
                var httpClient = new HttpClient();

                httpClient.BaseAddress = new Uri(string.Format("{0}{1}", BaseUrl, Version));

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                return httpClient;
            }
        }

        #endregion

        #region Authentication

        public string AccessToken
        {
            get { return (string)(ApplicationData.Current.LocalSettings.Values["token"]); }
            private set { ApplicationData.Current.LocalSettings.Values["token"] = value; }
        }

        public void TryAuthenticate(string token = null)
        {
            if (!string.IsNullOrWhiteSpace(token))
                AccessToken = token;
        }

        #endregion

        #region Rooms

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync("rooms");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Room>>(content);
                }

                throw new HttpRequestException();
            }
        }

        #endregion
    }
}
