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

        private readonly string _baseApiAddress = string.Format("{0}{1}", Constants.ApiBaseUrl, Constants.ApiVersion);
        private readonly string _baseStreamingApiAddress = string.Format("{0}{1}", Constants.StreamApiBaseUrl, Constants.ApiVersion);

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

        private readonly IApplicationStorageService _applicationStorageService;

        #endregion


        #region Constructor

        public GitterApiService(IApplicationStorageService applicationStorageService)
        {
            _applicationStorageService = applicationStorageService;
        }

        #endregion


        #region Authentication

        public string AccessToken
        {
            get { return (string)_applicationStorageService.Retrieve("token"); }
            private set { _applicationStorageService.Save("token", value); }
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
                var response = await httpClient.GetAsync(new Uri(_baseApiAddress + "user"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<User>>(content).FirstOrDefault();
                }

                throw new Exception();
            }
        }

        public async Task ReadChatMessagesAsync(string userId, string roomId, IEnumerable<string> messageIds)
        {
            using (var httpClient = HttpClient)
            {
                var content = new HttpStringContent("{\"chat\": " + JsonConvert.SerializeObject(messageIds) + "}",
                    UnicodeEncoding.Utf8, 
                    "application/json");

                var response = await httpClient.PostAsync(
                    new Uri(_baseApiAddress + string.Format("user/{0}/rooms/{1}/unreadItems", userId, roomId)),
                    content);

                if (response.IsSuccessStatusCode)
                    return;

                throw new Exception();
            }
        }

        #endregion

        #region Rooms

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.GetAsync(new Uri(_baseApiAddress + "rooms"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Room>>(content);
                }

                throw new Exception();
            }
        }

        public async Task<Room> JoinRoomAsync(string uri)
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(new Uri(_baseApiAddress + "rooms"),
                    new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"uri", uri}
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Room>(content);
                }

                throw new Exception();
            }
        }

        #endregion

        #region Messages

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            string url = string.Format("rooms/{0}/chatMessages", roomId);

            return Observable.Using(() => HttpClient,
                client => client.GetInputStreamAsync(new Uri(_baseStreamingApiAddress + url))
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

        public async Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null)
        {
            using (var httpClient = HttpClient)
            {
                string url = string.Format("rooms/{0}/chatMessages?limit={1}", roomId, limit);
                if (!string.IsNullOrWhiteSpace(beforeId))
                    url += string.Format("&beforeId={0}", beforeId);

                var response = await httpClient.GetAsync(new Uri(_baseApiAddress + url));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Message>>(content);
                }

                throw new Exception();
            }
        }

        public async Task<Message> SendMessageAsync(string roomId, string message)
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PostAsync(new Uri(_baseApiAddress + string.Format("rooms/{0}/chatMessages", roomId)),
                    new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"text", message}
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Message>(content);
                }

                throw new Exception();
            }
        }

        public async Task<Message> UpdateMessageAsync(string roomId, string messageId, string message)
        {
            using (var httpClient = HttpClient)
            {
                var response = await httpClient.PutAsync(new Uri(_baseApiAddress + string.Format("rooms/{0}/chatMessages/{1}", roomId, messageId)),
                    new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"text", message}
                    }));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Message>(content);
                }

                throw new Exception();
            }
        }

        #endregion
    }
}
