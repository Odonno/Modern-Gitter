using GitterSharp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitterSharp.Model;

namespace Gitter.UnitTests.Fakes
{
    public class FakeGitterApiServiceWithException : IGitterApiService
    {
        #region Fields

        private string _token;

        #endregion


        #region Methods

        public Task<User> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organization>> GetOrganizationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Repository>> GetRepositoriesAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null, string afterId = null, int skip = 0)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Room>> GetRoomsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetSingleRoomMessageAsync(string roomId, string messageId)
        {
            throw new NotImplementedException();
        }

        public Task<Room> JoinRoomAsync(string roomName)
        {
            throw new NotImplementedException();
        }

        public Task MarkUnreadChatMessagesAsync(string userId, string roomId, IEnumerable<string> messageIds)
        {
            throw new NotImplementedException();
        }

        public Task<UnreadItems> RetrieveUnreadChatMessagesAsync(string userId, string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<Message> SendMessageAsync(string roomId, string message)
        {
            throw new NotImplementedException();
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public Task<Message> UpdateMessageAsync(string roomId, string messageId, string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
