using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private Room _selectedRoom;
        public Room SelectedRoom
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

        private readonly ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages { get { return _messages; } }

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
            SelectRoomCommand = new RelayCommand<Room>(SelectRoom);


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

                SelectedRoom = Rooms.First();

                var malditogeek = new User
                {
                    Id = "53307734c3599d1de448e192",
                    Username = "malditogeek",
                    DisplayName = "Mauro Pompilio",
                    Url = "/malditogeek",
                    SmallAvatarUrl = "https://avatars.githubusercontent.com/u/14751?",
                    MediumAvatarUrl = "https://avatars.githubusercontent.com/u/14751?"
                };

                _messages = new ObservableCollection<Message>
                {
                    new Message
                    {
                        Id = "53316dc47bfc1a000000000f",
                        Text = "Hi @suprememoocow !",
                        Html =
                            "Hi <span data-link-type=\"mention\" data-screen-name=\"suprememoocow\" class=\"mention\">@suprememoocow</span> !",
                        SentDate = new DateTime(2014, 3, 25, 11, 51, 32),
                        EditedDate = null,
                        User = malditogeek,
                        ReadByCurrent = false,
                        ReadCount = 0,
                        Urls = new List<string>(),
                        Mentions = new List<Mention>
                        {
                            new Mention
                            {
                                ScreenName = "suprememoocow",
                                UserId = "53307831c3599d1de448e19a"
                            }
                        },
                        Issues = new List<Issue>(),
                        Version = 1
                    },
                    new Message
                    {
                        Id = "53316ec37bfc1a0000000011",
                        Text = "I've been working on #11, it'll be ready to ship soon",
                        Html =
                            "I&#39;ve been working on <span data-link-type=\"issue\" data-issue=\"11\" class=\"issue\">#11</span>, it&#39;ll be ready to ship soon",
                        SentDate = new DateTime(2014, 3, 25, 11, 55, 47),
                        EditedDate = null,
                        User = malditogeek,
                        ReadByCurrent = false,
                        ReadCount = 0,
                        Urls = new List<string>(),
                        Mentions = new List<Mention>(),
                        Issues = new List<Issue>
                        {
                            new Issue {Number = "11"}
                        },
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

        private void SelectRoom(Room room)
        {
            SelectedRoom = room;
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