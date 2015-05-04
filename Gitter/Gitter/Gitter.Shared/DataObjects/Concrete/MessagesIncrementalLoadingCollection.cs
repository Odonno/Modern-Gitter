using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gitter.DataObjects.Abstract;
using Gitter.Model;
using Gitter.ViewModel;

namespace Gitter.DataObjects.Concrete
{
    public class MessagesIncrementalLoadingCollection : IncrementalLoadingCollection<Message>
    {
        public string RoomId { get; private set; }


        public MessagesIncrementalLoadingCollection(string roomId)
        {
            RoomId = roomId;
            ItemsPerPage = 50;
            Ascendant = true;
        }


        protected override async Task<IEnumerable<Message>> LoadMoreItemsAsync()
        {
            string beforeId = Ascendant ? ((Page++ == 0) ? null : this.Last().Id) : ((Page++ == 0) ? null : this.First().Id);

            return await ViewModelLocator.GitterApi.GetRoomMessagesAsync(RoomId, ItemsPerPage, beforeId);
        }
    }
}
