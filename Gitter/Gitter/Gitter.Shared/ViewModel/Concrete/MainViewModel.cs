using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Gitter.ViewModel.Abstract;
using Gitter.Services.Abstract;
using GitterSharp.Model;
using GitterSharp.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Gitter.Configuration;
using GalaSoft.MvvmLight;

namespace Gitter.ViewModel.Concrete
{
    public sealed class MainViewModel : ViewModelBase, IMainViewModel
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
        private readonly ITelemetryService _telemetryService;

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
                RaisePropertyChanged();
            }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                RaisePropertyChanged();
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
            private set
            {
                _selectedRoom = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
                ExecuteSearch();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged();
                ((RelayCommand)(RefreshCommand)).RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Commands

        public ICommand SelectRoomCommand { get; }
        public ICommand ChatWithUsCommand { get; }
        public ICommand GoToAboutPageCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleSearchCommand { get; }

        #endregion


        #region Constructor

        public MainViewModel(IGitterApiService gitterApiService,
            ILocalNotificationService localNotificationService,
            IApplicationStorageService applicationStorageService,
            IProgressIndicatorService progressIndicatorService,
            IPasswordStorageService passwordStorageService,
            IEventService eventService,
            ITelemetryService telemetryService,
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
            _telemetryService = telemetryService;

            // Commands
            SelectRoomCommand = new RelayCommand<IRoomViewModel>(SelectRoom);
            ChatWithUsCommand = new RelayCommand(ChatWithUs, CanChatWithUs);
            GoToAboutPageCommand = new RelayCommand(GoToAboutPage);
            RefreshCommand = new RelayCommand(Refresh, () => !IsRefreshing);
            ToggleSearchCommand = new RelayCommand<bool>(ToggleSearch);

            // Properties
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
                }, gitterApiService, localNotificationService, progressIndicatorService, eventService, telemetryService, this));

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
                }, gitterApiService, localNotificationService, progressIndicatorService, eventService, telemetryService, this));

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
                }, gitterApiService, localNotificationService, progressIndicatorService, eventService, telemetryService, this));

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
                }, gitterApiService, localNotificationService, progressIndicatorService, eventService, telemetryService, this));

                SelectedRoom = Rooms.FirstOrDefault();

                foreach (var room in Rooms)
                    SearchedRooms.Add(room);
            }
            else
            {
                // Code runs "for real"

                // Events
                _eventService.ReadRoom
                    .Subscribe(room =>
                    {
                        HtmlToXaml.HtmlToXaml.RoomName = room.Room.Name;
                    });

                HtmlToXaml.HtmlToXaml.ImageTapped += (sender, args) =>
                {
                    var image = sender as Image;
                    var bitmapImage = image.Source as BitmapImage;

                    ViewModelLocator.FullImage.Source = bitmapImage.UriSource.OriginalString;
                    _navigationService.NavigateTo("FullImage");
                };

                // Retrieve access token to use in the app
                string token = _passwordStorageService.Retrieve("token");
                _gitterApiService.SetToken(token);

                // Add event that will update READ new messages
                _currentSelectedRoomUnreadMessages = _eventService.NotifyUnreadMessages
                    .Subscribe(async unreadMessages => await NotifyReadMessages(unreadMessages));
            }
        }

        #endregion


        #region Command Methods

        private void SelectRoom(IRoomViewModel room)
        {
            if (room == null)
            {
                UnselectRoom();
                return;
            }

            SelectedRoom = room;

            _telemetryService.TrackEvent("SelectRoom",
                new Dictionary<string, string> { { "Room", SelectedRoom.Room.Name } });

            if (!SelectedRoom.IsLoaded)
            {
                // Load room the first time
                SelectedRoom.Messages.Reset();
                SelectedRoom.IsLoaded = true;
            }

#if WINDOWS_PHONE_APP
            // Go to dedicated room
            _navigationService.NavigateTo("Room");

            // @Odonno, I am no longer sure what you are going to put in here when calling to clear the notifications
            // Can you let me know what you would be putting in here, as it will no longer be the id of the room
            _localNotificationService.ClearNotificationGroupAsync(SelectedRoom.Room.Name);
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

            // Retrieve joined room if it already exists
            var joinedRoom = Rooms.FirstOrDefault(r => r.Room.Name == OwnChatRoomName);
            bool alreadyJoinedRoom = (joinedRoom != null);

            // Join and add room if not already done
            if (!alreadyJoinedRoom)
                joinedRoom = await JoinRoomAsync(OwnChatRoomName);

            _telemetryService.TrackEvent("ChatWithUs",
                    new Dictionary<string, string> { { "AlreadyJoined", alreadyJoinedRoom.ToString() } });

            // Automatically select the room to chat
            SelectRoom(joinedRoom);

            // End async task
            await _progressIndicatorService.HideAsync();
        }

        private void GoToAboutPage()
        {
#if WINDOWS_PHONE_APP
            // Go to about page
            _navigationService.NavigateTo("About");
#endif
        }

        private async void Refresh()
        {
            await RefreshAsync();
        }

        private void ToggleSearch(bool toggle)
        {
            SearchedRoomText = string.Empty;
        }

        #endregion


        #region Private Methods

        private async Task<IRoomViewModel> JoinRoomAsync(string roomName)
        {
            // Join room through API
            var room = await _gitterApiService.JoinRoomAsync(roomName);

            // Add the room to the existing list
            var roomViewModel = new RoomViewModel(room, _gitterApiService, _localNotificationService, _progressIndicatorService, _eventService, _telemetryService, this);
            Rooms.Add(roomViewModel);

            return roomViewModel;
        }

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
                Rooms.Add(new RoomViewModel(room, _gitterApiService, _localNotificationService, _progressIndicatorService, _eventService, _telemetryService, this));

            // Execute search each time we refresh rooms
            ExecuteSearch();

            _eventService.RefreshRooms.OnNext(true);
        }

        private async Task NotifyReadMessages(IEnumerable<IMessageViewModel> unreadMessages)
        {
            // Notify only if there is unread messages
            if (unreadMessages.All(m => m.Read))
                return;

            try
            {
                // Update server to tell user read messages
                await _gitterApiService.MarkUnreadChatMessagesAsync(
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
                            message.MarkAsRead();
                            unreadCount++;
                        }

                        SelectedRoom.UnreadMessagesCount -= unreadCount;
                    });
            }
            catch (Exception ex)
            {
                _telemetryService.TrackException(ex);
                _localNotificationService.SendNotification("Error", "Can't validate reading new messages");
            }
        }

        private void ExecuteSearch()
        {
            IEnumerable<IRoomViewModel> rooms;

            if (string.IsNullOrWhiteSpace(SearchedRoomText))
            {
                rooms = Rooms;
            }
            else
            {
                string lowerSearch = SearchedRoomText.ToLower();
                rooms = Rooms.Where(room => room.Room.Name.ToLower().Contains(lowerSearch) ||
                                            room.Room.Url.ToLower().Contains(lowerSearch));
            }

            SearchedRooms.Clear();
            foreach (var room in rooms)
                SearchedRooms.Add(room);
        }

        #endregion


        #region Public Methods

        public async Task RefreshAsync()
        {
            // Start async task
            await _progressIndicatorService.ShowAsync();

            try
            {
                IsRefreshing = true;

                if (CurrentUser == null)
                {
                    CurrentUser = await _gitterApiService.GetCurrentUserAsync();

                    // Save Gitter User ID
                    _applicationStorageService.Save(StorageConstants.UserId, CurrentUser.Id);
                }

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

        public void UnselectRoom()
        {
            SelectedRoom = null;
        }

        public void OpenRealtimeStreams()
        {
            foreach (var room in Rooms)
                room.OpenRealtimeStream();
        }

        public void CloseRealtimeStreams()
        {
            foreach (var room in Rooms)
                room.CloseRealtimeStream();
        }

        #endregion
    }
}