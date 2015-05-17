using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Gitter.DataObjects.Abstract;
using Gitter.ViewModel;
using Gitter.ViewModel.Abstract;
using Gitter.ViewModel.Concrete;

namespace Gitter.DataObjects.Concrete
{
    public class MessagesIncrementalLoadingCollection : IncrementalLoadingCollection<IMessageViewModel>
    {
        private readonly object _lock = new object();

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

            lock (_lock)
            {
                if (Page++ == 0)
                    BeforeId = null;

                var beforeMessages = ViewModelLocator.GitterApi.GetRoomMessagesAsync(RoomId, ItemsPerPage, BeforeId)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

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
}
