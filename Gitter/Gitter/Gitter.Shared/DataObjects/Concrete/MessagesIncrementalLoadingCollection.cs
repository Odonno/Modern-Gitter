using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Gitter.DataObjects.Abstract;
using Gitter.Services.Abstract;
using Gitter.ViewModel;
using Gitter.ViewModel.Abstract;
using Gitter.ViewModel.Concrete;
using GitterSharp.Services;

namespace Gitter.DataObjects.Concrete
{
    public class MessagesIncrementalLoadingCollection : IncrementalLoadingCollection<IMessageViewModel>
    {
        #region Fields

        private readonly object _lock = new object();
        private readonly List<IMessageViewModel> _cachedMessages = new List<IMessageViewModel>();

        private readonly IMainViewModel _mainViewModel;

        #endregion


        #region Services

        private readonly IGitterApiService _gitterApiService;
        private readonly IEventService _eventService;

        #endregion


        #region Properties

        public string BeforeId { get; private set; }
        public string RoomId { get; private set; }

        #endregion


        #region Constructor


        public MessagesIncrementalLoadingCollection(string roomId,
            IGitterApiService gitterApiService,
            IEventService eventService,
            IMainViewModel mainViewModel)
        {
            // Properties
            RoomId = roomId;
            ItemsPerPage = 20;
            Ascendant = true;

            // View Models
            _mainViewModel = mainViewModel;

            // Services
            _gitterApiService = gitterApiService;
            _eventService = eventService;

            // Events
            _eventService.ReadRoom.Subscribe(async room =>
            {
                if (room.Room.Id == roomId)
                {
                    // Add current cached messages in UI
                    foreach (var message in _cachedMessages)
                        await AddItemAsync(message);

                    _cachedMessages.Clear();
                }
            });

            _eventService.PushMessage.Subscribe(async messageRoom =>
            {
                var id = messageRoom.Item1;
                var message = messageRoom.Item2;

                if (id == roomId)
                {
                    if (_mainViewModel.SelectedRoom != null)
                    {
                        // Add message in UI
                        await AddItemAsync(message);
                    }
                    else if (Page > 0) // only if room is already loaded
                    {
                        // Cached message to be added in the future
                        _cachedMessages.Add(message);
                    }
                }
            });
        }

        #endregion


        #region Methods

        protected override async Task<IEnumerable<IMessageViewModel>> LoadMoreItemsAsync()
        {
            if (ViewModelBase.IsInDesignModeStatic)
                return new List<IMessageViewModel>();

            // TODO : Use Semaphore
            lock (_lock)
            {
                if (Page++ == 0)
                    BeforeId = null;

                var beforeMessages = _gitterApiService.GetRoomMessagesAsync(RoomId, ItemsPerPage, BeforeId)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                if (beforeMessages.Count() < ItemsPerPage)
                    HasMoreItems = false;
                else
                    BeforeId = Ascendant ? beforeMessages.First().Id : beforeMessages.Last().Id;

                var loadedMessages = beforeMessages
                    .Where(message => !string.IsNullOrWhiteSpace(message.Text))
                    .Select(message => new MessageViewModel(message));

                _eventService.NotifyUnreadMessages.OnNext(loadedMessages.Where(message => !message.Read));

                return loadedMessages;
            }
        }

        public override async Task AddItemAsync(IMessageViewModel item)
        {
            await base.AddItemAsync(item);

            if (!item.Read && _mainViewModel.CurrentUser.Id != item.User.Id)
                _eventService.NotifyUnreadMessages.OnNext(new[] { item });
        }

        #endregion
    }
}
