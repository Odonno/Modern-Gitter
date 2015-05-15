using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Gitter.DataObjects.Abstract;
using Gitter.Model;
using Gitter.ViewModel;
using Gitter.ViewModel.Abstract;
using Gitter.ViewModel.Concrete;

namespace Gitter.DataObjects.Concrete
{
    public class MessagesIncrementalLoadingCollection : IncrementalLoadingCollection<IMessageViewModel>
    {
        public string RoomId { get; private set; }


        public MessagesIncrementalLoadingCollection(string roomId)
        {
            RoomId = roomId;
            ItemsPerPage = 50;
            Ascendant = true;
        }


        protected override async Task<IEnumerable<IMessageViewModel>> LoadMoreItemsAsync()
        {
#if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
                return new List<IMessageViewModel>();
#endif

            string beforeId = Ascendant ? ((Page++ == 0) ? null : this.Last().Id) : ((Page++ == 0) ? null : this.First().Id);

            return (await ViewModelLocator.GitterApi.GetRoomMessagesAsync(RoomId, ItemsPerPage, beforeId))
                .Select(message => new MessageViewModel(message));
        }
    }
}
