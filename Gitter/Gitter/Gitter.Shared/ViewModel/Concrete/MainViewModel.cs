using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Gitter.ViewModel.Abstract;
using Gitter.Services.Abstract;
using GitterSharp.Model;
using GitterSharp.Services;
using ReactiveUI;

namespace Gitter.ViewModel.Concrete
{
    public sealed class MainViewModel : ReactiveViewModelBase, IMainViewModel
    {
        #region Fields

        private const string OwnChatRoomName = "Odonno/Modern-Gitter";

        private IDisposable _currentSelectedRoomUnreadMessages;
        private IDisposable _refreshRooms;

        #endregion


        #region Services

        private readonly IGitterApiService _gitterApiService;
        private readonly ILocalNotificationService _localNotificationService;
        private readonly IApplicationStorageService _applicationStorageService;
        private readonly IProgressIndicatorService _progressIndicatorService;
        private readonly IPasswordStorageService _passwordStorageService;
        private readonly IEventService _eventService;
        private readonly INavigationService _navigationService;

        #endregion


        #region Properties

        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get
            {
                return _currentDateTime;
            }
            private set
            {
                _currentDateTime = value;
                this.RaisePropertyChanged();
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            private set
            {
                _currentUser = value;
                this.RaisePropertyChanged();
                ((RelayCommand)(ChatWithUsCommand)).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<IRoomViewModel> Rooms { get; } = new ObservableCollection<IRoomViewModel>();

        private IRoomViewModel _selectedRoom;
        public IRoomViewModel SelectedRoom
        {
            get
            {
                return _selectedRoom;
            }
            set
            {
                _selectedRoom = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<IRoomViewModel> SearchedRooms { get; } = new ObservableCollection<IRoomViewModel>();

        private string _searchedRoomText;
        public string SearchedRoomText
        {
            get
            {
                return _searchedRoomText;
            }
            set
            {
                _searchedRoomText = value;
                this.RaisePropertyChanged();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                this.RaisePropertyChanged();
                ((RelayCommand)(RefreshCommand)).RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Commands

        public ICommand SelectRoomCommand { get; }
        public ICommand ChatWithUsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleSearchCommand { get; }

        #endregion


        #region Private Commands

        private ReactiveCommand<IEnumerable<IRoomViewModel>> _search;

        #endregion


        #region Constructor

        public MainViewModel(IGitterApiService gitterApiService,
            ILocalNotificationService localNotificationService,
            IApplicationStorageService applicationStorageService,
            IProgressIndicatorService progressIndicatorService,
            IPasswordStorageService passwordStorageService,
            IEventService eventService,
            INavigationService navigationService)
        {
            // Services
            _gitterApiService = gitterApiService;
            _localNotificationService = localNotificationService;
            _applicationStorageService = applicationStorageService;
            _progressIndicatorService = progressIndicatorService;
            _passwordStorageService = passwordStorageService;
            _eventService = eventService;
            _navigationService = navigationService;

            // Commands
            SelectRoomCommand = new RelayCommand<IRoomViewModel>(SelectRoom);
            ChatWithUsCommand = new RelayCommand(ChatWithUs, CanChatWithUs);
            RefreshCommand = new RelayCommand(Refresh, () => !IsRefreshing);
            ToggleSearchCommand = new RelayCommand<bool>(ToggleSearch);

            // Create Rx commands
            CreateSearchCommand();

            // ViewModel properties
            CurrentDateTime = DateTime.Now;


            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                CurrentUser = new User
                {
                    Id = "53307734c3599d1de448e192",
                    Username = "malditogeek",
                    DisplayName = "Mauro Pompilio",
                    Url = "/malditogeek",
                    SmallAvatarUrl = "https://avatars.githubusercontent.com/u/14751?",
                    MediumAvatarUrl = "https://avatars.githubusercontent.com/u/14751?"
                };

                var suprememoocow = new User
                {
                    Id = "53307831c3599d1de448e19a",
                    Username = "suprememoocow",
                    DisplayName = "Andrew Newdigate",
                    Url = "/suprememoocow,",
                    SmallAvatarUrl = "https://avatars.githubusercontent.com/u/594566?",
                    MediumAvatarUrl = "https://avatars.githubusercontent.com/u/594566?"
                };

                Rooms.Add(new RoomViewModel(new Room
                {
                    Id = "53307860c3599d1de448e19d",
                    Name = "Andrew Newdigate",
                    Topic = string.Empty,
                    OneToOne = true,
                    Users = new[] { suprememoocow },
                    UnreadItems = 52,
                    UnreadMentions = 0,
                    DisabledNotifications = false,
                    Type = "ONETOONE"
                }));

                Rooms.Add(new RoomViewModel(new Room
                {
                    Id = "5330777dc3599d1de448e194",
                    Name = "gitterHQ",
                    Topic = "Gitter",
                    Url = "gitterHQ",
                    OneToOne = false,
                    UserCount = 2,
                    UnreadItems = 0,
                    UnreadMentions = 0,
                    LastAccessTime = new DateTime(2014, 3, 24, 18, 22, 28),
                    DisabledNotifications = false,
                    Type = "ORG",
                    Version = 1
                }));

                Rooms.Add(new RoomViewModel(new Room
                {
                    Id = "5330780dc3599d1de448e198",
                    Name = "gitterHQ/devops",
                    Topic = string.Empty,
                    Url = "gitterHQ/devops",
                    OneToOne = false,
                    UserCount = 2,
                    UnreadItems = 3,
                    UnreadMentions = 0,
                    LastAccessTime = new DateTime(2014, 3, 24, 18, 23, 10),
                    DisabledNotifications = false,
                    Type = "ORG_CHANNEL",
                    Version = 1
                }));

                Rooms.Add(new RoomViewModel(new Room
                {
                    Id = "53307793c3599d1de448e196",
                    Name = "malditogeek/vmux",
                    Topic = "VMUX - Plugin-free video calls in your browser using WebRTC",
                    Url = "gitterHQ/devops",
                    OneToOne = false,
                    UserCount = 2,
                    UnreadItems = 42,
                    UnreadMentions = 0,
                    LastAccessTime = new DateTime(2014, 3, 24, 18, 21, 08),
                    DisabledNotifications = false,
                    Type = "REPO",
                    Version = 1
                }));

                SelectedRoom = Rooms.FirstOrDefault();
            }
            else
            {
                // Code runs "for real"

                // Retrieve access token to use in the app
                string token = _passwordStorageService.Retrieve("token");
                _gitterApiService.TryAuthenticate(token);

                // Refresh the main menu by loading rooms
                Refresh();

                // Add event that will update READ new messages
                _currentSelectedRoomUnreadMessages = _eventService.NotifyUnreadMessages
                    .Subscribe(async unreadMessages => await NotifyReadMessages(unreadMessages));
            }
        }

        #endregion


        #region Command Methods

        private void SelectRoom(IRoomViewModel room)
        {
            // Select the Room
            if (SelectedRoom != room)
            {
                SelectedRoom = room;

                App.TelemetryClient.TrackEvent("SelectRoom",
                    new Dictionary<string, string> { { "Room", room.Room.Name } });
            }

            if (!SelectedRoom.IsLoaded)
            {
                // Load room the first time
                SelectedRoom.Messages.Reset();
                SelectedRoom.IsLoaded = true;
            }

#if WINDOWS_PHONE_APP
            // Go to dedicated room
            _navigationService.NavigateTo("Room");
#endif
        }

        private bool CanChatWithUs()
        {
            return CurrentUser != null;
        }
        private async void ChatWithUs()
        {
            // Start async task
            await _progressIndicatorService.ShowAsync();

            var alreadyJoinedRoom = Rooms.FirstOrDefault(r => r.Room.Name == OwnChatRoomName);

            if (alreadyJoinedRoom == null)
            {
                var room = await _gitterApiService.JoinRoomAsync(OwnChatRoomName);
                alreadyJoinedRoom = new RoomViewModel(room);
                Rooms.Add(alreadyJoinedRoom);

                App.TelemetryClient.TrackEvent("ChatWithUs",
                    new Dictionary<string, string> { { "AlreadyJoined", "false" } });
            }
            else
            {
                App.TelemetryClient.TrackEvent("ChatWithUs",
                    new Dictionary<string, string> { { "AlreadyJoined", "true" } });
            }

            SelectRoom(alreadyJoinedRoom);

            // End async task
            await _progressIndicatorService.HideAsync();
        }

        private async void Refresh()
        {
            // Start async task
            await _progressIndicatorService.ShowAsync();

            try
            {
                IsRefreshing = true;

                if (CurrentUser == null)
                    CurrentUser = await _gitterApiService.GetCurrentUserAsync();
                await RefreshRoomsAsync();
            }
            catch (Exception ex)
            {
                _localNotificationService.SendNotification("Network failure", "This app requires a network connection");
            }
            finally
            {
                IsRefreshing = false;
            }

            // End async task
            await _progressIndicatorService.HideAsync();
        }

        private void ToggleSearch(bool toggle)
        {
            if (!toggle)
                SearchedRoomText = string.Empty;
        }

        #endregion

        #region Rx Command Methods

        private void CreateSearchCommand()
        {
            // Execute search when user finished to type the search text
            _search = ReactiveCommand.CreateAsyncTask(
                Observable.Return(true),
                _ => Task.FromResult(ExecuteSearch()));

            // Execute Search when user stop to type > 0.5s
            this.WhenAnyValue(x => x.SearchedRoomText)
                .Throttle(TimeSpan.FromSeconds(0.5), RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x._search);

            // Update UI when we executed the search
            _search.Subscribe(rooms =>
            {
                SearchedRooms.Clear();
                foreach (var room in rooms)
                    SearchedRooms.Add(room);
            });
        }

        private IEnumerable<IRoomViewModel> ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchedRoomText))
                return Rooms;

            string lowerSearch = SearchedRoomText.ToLower();
            return Rooms.Where(room => room.Room.Name.ToLower().Contains(lowerSearch) ||
                                        room.Room.Url.ToLower().Contains(lowerSearch));
        }

        #endregion


        #region Methods

        private async Task RefreshRoomsAsync()
        {
            var rooms = await _gitterApiService.GetRoomsAsync();
            Rooms.Clear();

            // Order rooms (by favourites, unread mentions, unread items and then last accessed time)
            var orderedRooms = from room in rooms
                               orderby
                                   room.Favourite descending,
                                   room.UnreadMentions descending,
                                   room.UnreadItems descending,
                                   room.LastAccessTime descending
                               select room;

            // Add ordered rooms to UI list
            foreach (var room in orderedRooms)
                Rooms.Add(new RoomViewModel(room));

            // Execute search each time we refresh rooms
            await _search.ExecuteAsync();

            _eventService.RefreshRooms.OnNext(true);
        }

        public void SelectRoom(string roomName)
        {
            if (Rooms.Any())
            {
                var room = Rooms.FirstOrDefault(r => r.Room.Name == roomName);
                SelectRoom(room);
            }
            else
            {
                _refreshRooms = _eventService.RefreshRooms.Subscribe(_ =>
                {
                    var room = Rooms.FirstOrDefault(r => r.Room.Name == roomName);
                    SelectRoom(room);
                    _refreshRooms.Dispose();
                });
            }
        }

        private async Task NotifyReadMessages(IEnumerable<IMessageViewModel> unreadMessages)
        {
            // Notify only if there is unread messages
            if (unreadMessages.All(m => m.Read))
                return;

            try
            {
                // Update server to tell user read messages
                await _gitterApiService.ReadChatMessagesAsync(
                    CurrentUser.Id,
                    SelectedRoom.Room.Id,
                    unreadMessages.Select(m => m.Id));

                // Remove notification data cause there is no new unread message
                if (_applicationStorageService.Exists(SelectedRoom.Room.Name))
                    _applicationStorageService.Remove(SelectedRoom.Room.Name);

                // Update UI
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () =>
                    {
                        int unreadCount = 0;

                        foreach (var message in unreadMessages)
                        {
                            message.ReadByCurrent();
                            unreadCount++;
                        }

                        SelectedRoom.UnreadMessagesCount -= unreadCount;
                    });
            }
            catch (Exception ex)
            {
                App.TelemetryClient.TrackException(ex);
                _localNotificationService.SendNotification("Error", "Can't validate reading new messages");
            }
        }

        #endregion
    }
}