using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Gitter.API.Services.Abstract;
using Gitter.Messages;
using Gitter.Model;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public sealed class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region Fields

        private const string OwnChatRoomName = "Odonno/Modern-Gitter";

        private IDisposable _currentSelectedRoomUnreadMessages;

        #endregion


        #region Services

        private readonly IGitterApiService _gitterApiService;

        #endregion


        #region Properties

        public DateTime CurrentDateTime { get; private set; }
        public User CurrentUser { get; private set; }
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

        public MainViewModel(IGitterApiService gitterApiService)
        {
            // Services
            _gitterApiService = gitterApiService;

            // Commands
            SelectRoomCommand = new RelayCommand<IRoomViewModel>(SelectRoom);
            ChatWithUsCommand = new RelayCommand(ChatWithUs);

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
            }
            else
            {
                // Code runs "for real"

                LaunchAsync();
            }
        }

        #endregion


        #region Command Methods

        private void SelectRoom(IRoomViewModel room)
        {
            if (SelectedRoom != room)
            {
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
                        if (unreadMessages.Any())
                        {
                            await _gitterApiService.ReadChatMessagesAsync(CurrentUser.Id, SelectedRoom.Room.Id, unreadMessages.Select(m => m.Id));

                            foreach (var message in unreadMessages)
                                message.ReadByCurrent();
                        }
                    });

                // Update UI
                Messenger.Default.Send(new SelectRoomMessage());

                App.TelemetryClient.TrackEvent("SelectRoom",
                    new Dictionary<string, string> { { "Room", room.Room.Name } });
            }
        }

        private async void ChatWithUs()
        {
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
        }

        #endregion


        #region Methods

        private async Task LaunchAsync()
        {
            CurrentUser = await _gitterApiService.GetCurrentUserAsync();
        }

        #endregion
    }
}