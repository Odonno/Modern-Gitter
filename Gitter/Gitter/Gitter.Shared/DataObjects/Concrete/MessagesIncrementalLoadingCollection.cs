using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
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
        #region Fields
        private readonly object _lock = new object();

        #endregion


        #region Properties

        public string BeforeId { get; private set; }
        public string RoomId { get; private set; }

        private readonly Subject<IEnumerable<IMessageViewModel>> _notifyUnreadMessages = new Subject<IEnumerable<IMessageViewModel>>();
        public IObservable<IEnumerable<IMessageViewModel>> NotifyUnreadMessages { get { return _notifyUnreadMessages; } }

        #endregion


        #region Constructor


        public MessagesIncrementalLoadingCollection(string roomId)
        {
            RoomId = roomId;
            ItemsPerPage = 20;
            Ascendant = true;
        }

        #endregion


        #region Methods

        protected override async Task<IEnumerable<IMessageViewModel>> LoadMoreItemsAsync()
        {
            if (ViewModelBase.IsInDesignModeStatic)
                return new List<IMessageViewModel>();

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

                var loadedMessages = beforeMessages
                    .Where(message => !string.IsNullOrWhiteSpace(message.Text))
                    .Select(message => new MessageViewModel(message));

                _notifyUnreadMessages.OnNext(loadedMessages.Where(message => !message.Read));

                return loadedMessages;
            }
        }

        public override async Task AddItemAsync(IMessageViewModel item)
        {
            await base.AddItemAsync(item);

            if (!item.Read && ViewModelLocator.Main.CurrentUser.Id != item.User.Id)
                _notifyUnreadMessages.OnNext(new[] { item });
        }

        #endregion
    }
}
