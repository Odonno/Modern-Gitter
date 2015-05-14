using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
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
#if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
                return new List<Message>();
#endif

            string beforeId = Ascendant ? ((Page++ == 0) ? null : this.Last().id) : ((Page++ == 0) ? null : this.First().id);

            return await ViewModelLocator.GitterApi.GetRoomMessagesAsync(RoomId, ItemsPerPage, beforeId);
        }
    }
}
