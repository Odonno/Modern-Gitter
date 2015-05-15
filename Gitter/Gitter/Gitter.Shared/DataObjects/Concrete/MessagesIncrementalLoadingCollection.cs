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
        public string BeforeId { get; private set; }
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
            
            if (Page++ == 0)
                BeforeId = null;

            var beforeMessages = await ViewModelLocator.GitterApi.GetRoomMessagesAsync(RoomId, ItemsPerPage, BeforeId);

            if (beforeMessages.Count() < ItemsPerPage)
                HasMoreItems = false;
            else
                BeforeId = Ascendant ? beforeMessages.First().Id : beforeMessages.Last().Id;

            return beforeMessages
                .Where(message => !string.IsNullOrWhiteSpace(message.Text))
                .Select(message => new MessageViewModel(message));
        }
    }
}
