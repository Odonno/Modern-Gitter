using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Gitter.API.Services.Abstract;
using Gitter.Model;
using Gitter.ViewModel.Abstract;
using Gitter.Services.Abstract;

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
            private set
            {
                _currentUser = value;
                RaisePropertyChanged();
                ((RelayCommand)(ChatWithUsCommand)).RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<IRoomViewModel> _rooms = new ObservableCollection<IRoomViewModel>();
        public ObservableCollection<IRoomViewModel> Rooms { get { return _rooms; } }

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
                RaisePropertyChanged();
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

        public ICommand SelectRoomCommand { get; private set; }
        public ICommand ChatWithUsCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        #endregion


        #region Constructor

        public MainViewModel(IGitterApiService gitterApiService,
            ILocalNotificationService localNotificationService,
            IApplicationStorageService applicationStorageService,
            IProgressIndicatorService progressIndicatorService,
            IEventService eventService,
            INavigationService navigationService)
        {
            // Services
            _gitterApiService = gitterApiService;
            _localNotificationService = localNotificationService;
            _applicationStorageService = applicationStorageService;
            _progressIndicatorService = progressIndicatorService;
            _eventService = eventService;
            _navigationService = navigationService;

            // Commands
            SelectRoomCommand = new RelayCommand<IRoomViewModel>(SelectRoom);
            ChatWithUsCommand = new RelayCommand(ChatWithUs, CanChatWithUs);
            RefreshCommand = new RelayCommand(Refresh, () => !IsRefreshing);

            // ViewModels
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
                    Users = new[] {suprememoocow},
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

                Refresh();

                // Add event that will update READ new messages
                _currentSelectedRoomUnreadMessages = _eventService.NotifyUnreadMessages.Subscribe(
                    async unreadMessages =>
                    {
                        // Notify only if there is unread messages
                        if (!unreadMessages.Any(m => !m.Read))
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
                    });
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

        #endregion


        #region Methods

        private async Task RefreshRoomsAsync()
        {
            var rooms = await _gitterApiService.GetRoomsAsync();

            Rooms.Clear();

            foreach (var room in rooms)
                Rooms.Add(new RoomViewModel(room));

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

        #endregion
    }
}