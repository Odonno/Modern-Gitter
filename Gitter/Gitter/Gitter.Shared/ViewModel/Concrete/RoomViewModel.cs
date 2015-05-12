using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gitter.API.Services.Abstract;
using Gitter.DataObjects.Concrete;
using Gitter.Model;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class RoomViewModel : ViewModelBase, IRoomViewModel
    {
        #region Services

        private readonly IGitterApiService _gitterApiService;

        #endregion


        #region Properties

        public Room Room { get; private set; }

        private readonly MessagesIncrementalLoadingCollection _messages;
        public MessagesIncrementalLoadingCollection Messages { get { return _messages; } }

        private string _textMessage;
        public string TextMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                RaisePropertyChanged();
                ((RelayCommand)SendMessageCommand).RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Commands

        public ICommand SendMessageCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        #endregion


        #region Constructor

        public RoomViewModel(Room room)
        {
            // Properties
            Room = room;

            // Commands
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            RefreshCommand = new RelayCommand(Refresh);

            // Inject Services
            _gitterApiService = ViewModelLocator.GitterApi;


            /*if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                var malditogeek = new User
                {
                    Id = "53307734c3599d1de448e192",
                    Username = "malditogeek",
                    DisplayName = "Mauro Pompilio",
                    Url = "/malditogeek",
                    SmallAvatarUrl = "https://avatars.githubusercontent.com/u/14751?",
                    MediumAvatarUrl = "https://avatars.githubusercontent.com/u/14751?"
                };

                _messages = new MessagesIncrementalLoadingCollection("123456")
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
                        Urls = new List<MessageUrl>(),
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
                        Urls = new List<MessageUrl>(),
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
            {*/
                // Code runs "for real"

                _messages = new MessagesIncrementalLoadingCollection(Room.id);
            //}

            // Use the stream API to add new messages when they comes
            _gitterApiService.GetRealtimeMessages(Room.id).Subscribe(async message =>
            {
                await Messages.AddItem(message);
            });
        }

        #endregion


        #region Command Methods

        private bool CanSendMessage()
        {
            return Room != null && !string.IsNullOrWhiteSpace(TextMessage);
        }
        private async void SendMessage()
        {
            await _gitterApiService.SendMessage(Room.id, TextMessage);

            App.TelemetryClient.TrackEvent("SendMessage",
                new Dictionary<string, string> { { "Room", Room.name } },
                new Dictionary<string, double> { { "MessageLength", TextMessage.Length } });
            TextMessage = string.Empty;
        }

        #endregion


        #region Methods

        public void Refresh()
        {
            Messages.Reset();

            App.TelemetryClient.TrackEvent("RefreshRoom",
                new Dictionary<string, string> { { "Room", Room.name } });
        }

        #endregion
    }
}
