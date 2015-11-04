using GitterSharp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitterSharp.Model;

namespace Gitter.UnitTests.Fakes
{
    public class FakeGitterApiServiceWithResult : IGitterApiService
    {
        #region Fake Fields

        private string _token;

        #endregion


        #region Methods

        public Task<User> GetCurrentUserAsync()
        {
            var user = new User
            {
                Id = "53307734c3599d1de448e192",
                Username = "malditogeek",
                DisplayName = "Mauro Pompilio",
                Url = "/malditogeek",
                SmallAvatarUrl = "https://avatars.githubusercontent.com/u/14751?",
                MediumAvatarUrl = "https://avatars.githubusercontent.com/u/14751?"
            };
            return Task.FromResult(user);
        }

        public Task<IEnumerable<Organization>> GetOrganizationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public IObservable<Message> GetRealtimeMessages(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Repository>> GetRepositoriesAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetRoomMessagesAsync(string roomId, int limit = 50, string beforeId = null, string afterId = null, int skip = 0)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Room>> GetRoomsAsync()
        {
            var suprememoocow = new User
            {
                Id = "53307831c3599d1de448e19a",
                Username = "suprememoocow",
                DisplayName = "Andrew Newdigate",
                Url = "/suprememoocow,",
                SmallAvatarUrl = "https://avatars.githubusercontent.com/u/594566?",
                MediumAvatarUrl = "https://avatars.githubusercontent.com/u/594566?"
            };

            IEnumerable<Room> rooms = new List<Room>
            {
                new Room
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
                    Type = "ORG",
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
                    UnreadItems = 3,
                    UnreadMentions = 0,
                    LastAccessTime = new DateTime(2014, 3, 24, 18, 23, 10),
                    DisabledNotifications = false,
                    Type = "ORG_CHANNEL",
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
                    UnreadItems = 42,
                    UnreadMentions = 0,
                    LastAccessTime = new DateTime(2014, 3, 24, 18, 21, 08),
                    DisabledNotifications = false,
                    Type = "REPO",
                    Version = 1
                }
            };
            return Task.FromResult(rooms);
        }

        public Task<Message> GetSingleRoomMessageAsync(string roomId, string messageId)
        {
            throw new NotImplementedException();
        }

        public Task<Room> JoinRoomAsync(string roomName)
        {
            throw new NotImplementedException();
        }

        public Task MarkUnreadChatMessagesAsync(string userId, string roomId, IEnumerable<string> messageIds)
        {
            throw new NotImplementedException();
        }

        public Task<UnreadItems> RetrieveUnreadChatMessagesAsync(string userId, string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<Message> SendMessageAsync(string roomId, string message)
        {
            throw new NotImplementedException();
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        public Task<Message> UpdateMessageAsync(string roomId, string messageId, string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}