using System;
using System.Collections.Generic;
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


        public IRoomsViewModel Rooms { get; private set; }

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

        #endregion


        #region Commands

        public ICommand SelectRoomCommand { get; private set; }
        public ICommand ChatWithUsCommand { get; private set; }

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

            // ViewModels
            Rooms = ViewModelLocator.Rooms;
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

                SelectedRoom = Rooms.Rooms.FirstOrDefault();
            }
            else
            {
                // Code runs "for real"

                LaunchAsync();
            }
        }

        #endregion


        #region Command Methods

        private async void SelectRoom(IRoomViewModel room)
        {
            // TODO : Optimize room management to not load room every time
            if (SelectedRoom != room)
            {
                // Start async task
                await _progressIndicatorService.ShowAsync();

                // Remove event that was updating READ new messages
                if (_currentSelectedRoomUnreadMessages != null)
                    _currentSelectedRoomUnreadMessages.Dispose();

                // Select the Room and update ViewModel
                SelectedRoom = room;
                SelectedRoom.Messages.Reset();

                // Add event that will update READ new messages
                _currentSelectedRoomUnreadMessages = SelectedRoom.Messages.NotifyUnreadMessages.Subscribe(
                    async unreadMessages =>
                    {
                        if (!unreadMessages.Any())
                            return;

                        try
                        {
                            await _gitterApiService.ReadChatMessagesAsync(
                                CurrentUser.Id,
                                SelectedRoom.Room.Id,
                                unreadMessages.Select(m => m.Id));

                            int unreadCount = 0;

                            foreach (var message in unreadMessages)
                            {
                                message.ReadByCurrent();
                                unreadCount++;
                            }

                            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                            await dispatcher.RunAsync(CoreDispatcherPriority.High, () => SelectedRoom.UnreadMessagesCount -= unreadCount);
                        }
                        catch (Exception ex)
                        {
                            App.TelemetryClient.TrackException(ex);
                            _localNotificationService.SendNotification("Error", "Can't validate reading new messages");
                        }
                    });

                // Remove notification data cause there is no new unread message
                if (_applicationStorageService.Exists(SelectedRoom.Room.Name))
                    _applicationStorageService.Remove(SelectedRoom.Room.Name);
                
                App.TelemetryClient.TrackEvent("SelectRoom",
                    new Dictionary<string, string> { { "Room", room.Room.Name } });

                // End async task
                await _progressIndicatorService.HideAsync();
            }

            // Go to dedicated room
            _navigationService.NavigateTo("Room");
        }

        private bool CanChatWithUs()
        {
            return CurrentUser != null;
        }
        private async void ChatWithUs()
        {
            // Start async task
            await _progressIndicatorService.ShowAsync();

            var alreadyJoinedRoom = Rooms.Rooms.FirstOrDefault(r => r.Room.Name == OwnChatRoomName);

            if (alreadyJoinedRoom == null)
            {
                var room = await _gitterApiService.JoinRoomAsync(OwnChatRoomName);
                alreadyJoinedRoom = new RoomViewModel(room);
                Rooms.Rooms.Add(alreadyJoinedRoom);

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

        #endregion


        #region Methods

        private async void LaunchAsync()
        {
            // Start async task
            await _progressIndicatorService.ShowAsync();

            try
            {
                CurrentUser = await _gitterApiService.GetCurrentUserAsync();
                await RefreshRoomsAsync();
            }
            catch (Exception ex)
            {
                _localNotificationService.SendNotification("Network failure", "This app requires a network connection");
            }

            // End async task
            await _progressIndicatorService.HideAsync();
        }

        private async Task RefreshRoomsAsync()
        {
            var rooms = await _gitterApiService.GetRoomsAsync();

            foreach (var room in rooms)
                Rooms.Rooms.Add(new RoomViewModel(room));

            _eventService.RefreshRooms.OnNext(true);
        }

        public void SelectRoom(string roomName)
        {
            if (Rooms.Rooms.Any())
            {
                var room = Rooms.Rooms.FirstOrDefault(r => r.Room.Name == roomName);
                SelectRoom(room);
            }
            else
            {
                _refreshRooms = _eventService.RefreshRooms.Subscribe(_ =>
                {
                    var room = Rooms.Rooms.FirstOrDefault(r => r.Room.Name == roomName);
                    SelectRoom(room);
                    _refreshRooms.Dispose();
                });
            }
        }

        #endregion
    }
}