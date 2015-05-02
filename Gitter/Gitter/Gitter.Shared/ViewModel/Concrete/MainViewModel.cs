using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gitter.API.Services.Abstract;
using Gitter.Model;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region Services

        private readonly IGitterApiService _gitterApiService;

        #endregion


        #region Properties

        private readonly ObservableCollection<Room> _rooms = new ObservableCollection<Room>();
        public ObservableCollection<Room> Rooms { get { return _rooms; } }

        #endregion


        #region Commands

        public ICommand SelectRoomCommand { get; private set; }

        #endregion


        #region Constructor

        public MainViewModel(IGitterApiService gitterApiService)
        {
            // Inject Seervices
            _gitterApiService = gitterApiService;

            // Commands
            SelectRoomCommand = new RelayCommand(SelectRoom);


            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                var suprememoocow = new User
                {
                    Id = "53307831c3599d1de448e19a",
                    Username = "suprememoocow",
                    DisplayName = "Andrew Newdigate",
                    Url = "/suprememoocow,",
                    SmallAvatarUrl = "https://avatars.githubusercontent.com/u/594566?",
                    MediumAvatarUrl = "https://avatars.githubusercontent.com/u/594566?"
                };

                _rooms = new ObservableCollection<Room>
                {
                    new Room
                    {
                        Id = "53307860c3599d1de448e19d",
                        Name = "Andrew Newdigate",
                        Topic = string.Empty,
                        OneToOne = true,
                        Users = new[] {suprememoocow},
                        UnreadItems = 0,
                        UnreadMentions = 0,
                        DisabledNotifications = false,
                        Type = RoomType.OneToOne
                    },
                    new Room
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
                        Type = RoomType.Organization,
                        Version = 1
                    },
                    new Room
                    {
                        Id = "5330780dc3599d1de448e198",
                        Name = "gitterHQ/devops",
                        Topic = string.Empty,
                        Url = "gitterHQ/devops",
                        OneToOne = false,
                        UserCount = 2,
                        UnreadItems = 0,
                        UnreadMentions = 0,
                        LastAccessTime = new DateTime(2014, 3, 24, 18, 23, 10),
                        DisabledNotifications = false,
                        Type = RoomType.OrganizationChannel,
                        Version = 1
                    },
                    new Room
                    {
                        Id = "53307793c3599d1de448e196",
                        Name = "malditogeek/vmux",
                        Topic = "VMUX - Plugin-free video calls in your browser using WebRTC",
                        Url = "gitterHQ/devops",
                        OneToOne = false,
                        UserCount = 2,
                        UnreadItems = 0,
                        UnreadMentions = 0,
                        LastAccessTime = new DateTime(2014, 3, 24, 18, 21, 08),
                        DisabledNotifications = false,
                        Type = RoomType.Repository,
                        Version = 1
                    }
                };
            }
            else
            {
                // Code runs "for real"

                Refresh();
            }
        }

        #endregion


        #region Command Methods

        private void SelectRoom()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Methods

        private async Task Refresh()
        {
            var rooms = await _gitterApiService.GetRoomsAsync();

            foreach (var room in rooms)
            {
                Rooms.Add(room);
            }
        }

        #endregion
    }
}