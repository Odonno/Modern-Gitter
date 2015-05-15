using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gitter.API.Services.Abstract;
using Gitter.Model;

namespace Gitter.API.Services.Concrete
{
    public class GitterApiService : IGitterApiService
    {
        #region Authentication

        public string AccessToken { get; private set; }

        public void TryAuthenticate(string token = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User

        public Task<User> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Rooms

        public Task<IEnumerable<Room>> GetRoomsAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Messages

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null)
        {
            throw new NotImplementedException();
        }

        public Task<Message> SendMessageAsync(string roomId, string message)
        {
            throw new NotImplementedException();
        }

        public Task<Message> UpdateMessageAsync(string roomId, string messageId, string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
