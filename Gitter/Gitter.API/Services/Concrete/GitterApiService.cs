using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Gitter.API.Configuration;
using Gitter.API.Services.Abstract;
using Gitter.Model;
using Gitter.Services.Abstract;
using Newtonsoft.Json;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace Gitter.API.Services.Concrete
{
    public class GitterApiService : IGitterApiService
    {
        #region Fields

        private readonly string _baseApiAddress = $"{Constants.ApiBaseUrl}{Constants.ApiVersion}";
        private readonly string _baseStreamingApiAddress = $"{Constants.StreamApiBaseUrl}{Constants.ApiVersion}";

        private HttpClient HttpClient
        {
            get
            {
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(AccessToken))
                    httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", AccessToken);

                return httpClient;
            }
        }

        #endregion


        #region Services

        private readonly IPasswordStorageService _passwordStorageService;

        #endregion


        #region Constructor

        public GitterApiService(IPasswordStorageService passwordStorageService)
        {
            _passwordStorageService = passwordStorageService;
        }

        #endregion


        #region Authentication

        public string AccessToken
        {
            get { return _passwordStorageService.Retrieve("token"); }
            private set { _passwordStorageService.Save("token", value); }
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
            string url = _baseApiAddress + "user";

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync(new Uri(url));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<User>>(result).FirstOrDefault();
                }

                throw new Exception();
            }
        }

        public async Task ReadChatMessagesAsync(string userId, string roomId, IEnumerable<string> messageIds)
        {
            string url = _baseApiAddress + $"user/{userId}/rooms/{roomId}/unreadItems";
            var content = new HttpStringContent("{\"chat\": " + JsonConvert.SerializeObject(messageIds) + "}",
                UnicodeEncoding.Utf8,
                "application/json");

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(new Uri(url), content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception();
            }
        }

        #endregion

        #region Rooms

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            string url = _baseApiAddress + "rooms";

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync(new Uri(url));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Room>>(result);
                }

                throw new Exception();
            }
        }

        public async Task<Room> JoinRoomAsync(string uri)
        {
            string url = _baseApiAddress + "rooms";
            var content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
            {
                {"uri", uri}
            });

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(new Uri(url), content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Room>(result);
                }

                throw new Exception();
            }
        }

        #endregion

        #region Messages

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            string url = _baseStreamingApiAddress + $"rooms/{roomId}/chatMessages";

            return Observable.Using(() => HttpClient,
                client => client.GetInputStreamAsync(new Uri(url))
                    .AsTask()
                    .ToObservable()
                    .Select(x => Observable.FromAsync(() => ReadStream(x.AsStreamForRead())).Repeat())
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

        public async Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null, string afterId = null, int skip = 0)
        {
            string url = _baseApiAddress + $"rooms/{roomId}/chatMessages?limit={limit}";

            if (!string.IsNullOrWhiteSpace(beforeId))
                url += $"&beforeId={beforeId}";

            if (!string.IsNullOrWhiteSpace(afterId))
                url += $"&afterId={afterId}";

            if (skip > 0)
                url += $"&skip={skip}";

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync(new Uri(url));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Message>>(result);
                }

                throw new Exception();
            }
        }

        public async Task<Message> SendMessageAsync(string roomId, string message)
        {
            string url = _baseApiAddress + $"rooms/{roomId}/chatMessages";
            var content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
            {
                {"text", message}
            });

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(new Uri(url), content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Message>(result);
                }

                throw new Exception();
            }
        }

        public async Task<Message> UpdateMessageAsync(string roomId, string messageId, string message)
        {
            string url = _baseApiAddress + $"rooms/{roomId}/chatMessages/{messageId}";
            var content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
            {
                {"text", message}
            });

            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PutAsync(new Uri(url), content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Message>(result);
                }

                throw new Exception();
            }
        }

        #endregion
    }
}
