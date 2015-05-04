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

        #region Rooms

        Task<IEnumerable<Room>> GetRoomsAsync();
        
        #endregion

        #region Messages

        Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null);
        Task<Message> SendMessage(string roomId, string message);

        #endregion
    }
}
