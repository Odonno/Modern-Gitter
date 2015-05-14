using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Gitter.API.Services.Abstract;
using Gitter.Model;
using ModernHttpClient;
using Newtonsoft.Json;

namespace Gitter.API.Services.Concrete
{
    public class GitterApiService : IGitterApiService
    {
        #region Fields

        private const string BaseUrl = "https://api.gitter.im/";
        private const string StreamBaseUrl = "https://stream.gitter.im/";
        private const string Version = "v1/";

        private HttpClient HttpClient
        {
            get
            {
                var httpClient = new HttpClient(new NativeMessageHandler());

                httpClient.BaseAddress = new Uri(string.Format("{0}{1}", BaseUrl, Version));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                return httpClient;
            }
        }

        private HttpClient StreamHttpClient
        {
            get
            {
                var httpClient = new HttpClient(new NativeMessageHandler());

                httpClient.BaseAddress = new Uri(string.Format("{0}{1}", StreamBaseUrl, Version));
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

        #region User

        public async Task<User> GetCurrentUserAsync()
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync("user");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<User>>(content).FirstOrDefault();
                }

                throw new HttpRequestException();
            }
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

        #region Messages

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            string url = string.Format("rooms/{0}/chatMessages", roomId);

            return Observable.Using(() => StreamHttpClient,
                client => client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    .ToObservable()
                    .SelectMany(x => x.Content.ReadAsStreamAsync())
                    .Select(x => Observable.FromAsync(() => ReadStream(x)).Repeat())
                    .Concat()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(JsonConvert.DeserializeObject<Message>));
        }
        private async Task<string> ReadStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            {
                return await reader.ReadLineAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null)
        {
            using (var httpClient = HttpClient)
            {
                string url = string.Format("rooms/{0}/chatMessages?limit={1}", roomId, limit);
                if (!string.IsNullOrWhiteSpace(beforeId))
                    url += string.Format("&beforeId={0}", beforeId);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Message>>(content);
                }

                throw new HttpRequestException();
            }
        }

        public async Task<Message> SendMessageAsync(string roomId, string message)
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(string.Format("rooms/{0}/chatMessages", roomId),
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"text", message}
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Message>(content);
                }

                throw new HttpRequestException();
            }
        }

        #endregion
    }
}
