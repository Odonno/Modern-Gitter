using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gitter.Model;

namespace Gitter.API.Services.Abstract
{
    public interface IGitterApiService
    {
        #region Authentication

        string AccessToken { get; }
        void TryAuthenticate(string token = null);

        #endregion

        #region User

        Task<User> GetCurrentUserAsync();

        #endregion

        #region Rooms

        Task<IEnumerable<Room>> GetRoomsAsync();

        #endregion

        #region Messages

        IObservable<Message> GetRealtimeMessages(string roomId);
        Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null);
        Task<Message> SendMessageAsync(string roomId, string message);
        Task<Message> UpdateMessageAsync(string roomId, string messageId, string message);

        #endregion
    }
}
