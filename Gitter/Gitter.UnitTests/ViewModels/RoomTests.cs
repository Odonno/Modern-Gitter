using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Abstract;
using Gitter.ViewModel.Concrete;
using GitterSharp.Model;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class RoomTests
    {
        // TODO : Test property IsLoaded

        #region Fields

        private FakeGitterApiServiceWithResult _gitterApiService;
        private FakeLocalNotificationService _localNotificationService;
        private FakeProgressIndicatorService _progressIndicatorService;
        private EventService _eventService;
        private FakeApplicationStorageService _applicationStorageService;
        private FakePasswordStorageService _passwordStorageService;
        private FakeTelemetryService _telemetryService;
        private FakeNavigationService _navigationService;

        private IMainViewModel _mainViewModel;
        private IRoomViewModel _roomViewModel;

        #endregion


        #region Initialize

        public void TestInitialize(Room room)
        {
            _gitterApiService = new FakeGitterApiServiceWithResult();
            _localNotificationService = new FakeLocalNotificationService();
            _progressIndicatorService = new FakeProgressIndicatorService();
            _eventService = new EventService();
            _applicationStorageService = new FakeApplicationStorageService();
            _passwordStorageService = new FakePasswordStorageService();
            _telemetryService = new FakeTelemetryService();
            _navigationService = new FakeNavigationService();

            _mainViewModel = new MainViewModel(
                _gitterApiService,
                _localNotificationService,
                _applicationStorageService,
                _progressIndicatorService,
                _passwordStorageService,
                _eventService,
                _telemetryService,
                _navigationService);

            _mainViewModel.CurrentUser = _gitterApiService.GetCurrentUserAsync().Result;

            _roomViewModel = new RoomViewModel(room,
                _gitterApiService,
                _localNotificationService,
                _progressIndicatorService,
                _eventService,
                _telemetryService,
                _mainViewModel);
        }

        #endregion


        #region Methods

        [Fact]
        public void CreateRoom_Should_SetDefaultProperties()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act

            // Assert
            Assert.Same(room, _roomViewModel.Room);
            Assert.NotNull(_roomViewModel.Messages);
            Assert.Equal(0, _roomViewModel.Messages.Count);
            Assert.Equal(14, _roomViewModel.UnreadMessagesCount);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("    ", false)]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("    ", true)]
        [InlineData("a new message", true)]
        public void IncorrectValuesOfTextMessageAndIsSending_Should_NotEnableSendMessage(string textMessage, bool isSendingMessage)
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.TextMessage = textMessage;
            _roomViewModel.IsSendingMessage = isSendingMessage;
            bool result = _roomViewModel.SendMessageCommand.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("a new message", false)]
        public void CorrectValuesOfTextMessageAndIsSending_Should_EnableSendMessage(string textMessage, bool isSendingMessage)
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.TextMessage = textMessage;
            _roomViewModel.IsSendingMessage = isSendingMessage;
            bool result = _roomViewModel.SendMessageCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Should_CorrectlySendNewMessage()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.TextMessage = "A new message";
            _roomViewModel.IsSendingMessage = false;
            _roomViewModel.SendMessageCommand.Execute(null);

            await Task.Delay(200);

            // Assert
            Assert.False(_roomViewModel.IsSendingMessage);
            Assert.Equal(1, _gitterApiService.MessagesSent);
            Assert.Equal(string.Empty, _roomViewModel.TextMessage);
            Assert.Equal(1, _telemetryService.EventsTracked);
        }

        [Fact]
        public void SendingMessageWithoutEnterKeyPressedParam_Should_DoNothing()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.TextMessage = "A new message";
            _roomViewModel.IsSendingMessage = false;
            _roomViewModel.SendMessageWithParamCommand.Execute(false);

            // Assert
            Assert.Equal(0, _gitterApiService.MessagesSent);
            Assert.Equal("A new message", _roomViewModel.TextMessage);
            Assert.Equal(0, _telemetryService.EventsTracked);
        }

        [Fact]
        public async Task SendingMessageWithEnterKeyPressedParam_Should_SendMessageCorrectly()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.TextMessage = "A new message";
            _roomViewModel.IsSendingMessage = false;
            _roomViewModel.SendMessageWithParamCommand.Execute(true);

            await Task.Delay(200);

            // Assert
            Assert.Equal(1, _gitterApiService.MessagesSent);
            Assert.Equal(string.Empty, _roomViewModel.TextMessage);
            Assert.Equal(1, _telemetryService.EventsTracked);
        }

        [Fact]
        public void NullMessageViewModel_Should_NotEnableRemoveMessage()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            bool result = _roomViewModel.RemoveMessageCommand.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData("abcdef", 0)]
        [InlineData(null, 5)]
        [InlineData("abcdef", 5)]
        [InlineData(null, 10)]
        [InlineData("abcdef", 10)]
        [InlineData("53307734c3599d1de448e192", 10)]
        [InlineData(null, 15)]
        [InlineData("abcdef", 15)]
        [InlineData("53307734c3599d1de448e192", 15)]
        public void IncorrectValuesOfMessageViewModel_Should_NotEnableRemoveMessage(string userId, int minutesLeft)
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "123456",
                User = new User
                {
                    Id = userId
                },
                SentDate = _mainViewModel.CurrentDateTime.Subtract(TimeSpan.FromMinutes(minutesLeft))
            };
            var messageViewModel = new MessageViewModel(message);
            bool result = _roomViewModel.RemoveMessageCommand.CanExecute(messageViewModel);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("53307734c3599d1de448e192", 0)]
        [InlineData("53307734c3599d1de448e192", 5)]
        public void CorrectValuesOfMessageViewModel_Should_EnableRemoveMessage(string userId, int minutesLeft)
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "123456",
                User = new User
                {
                    Id = userId
                },
                SentDate = _mainViewModel.CurrentDateTime.Subtract(TimeSpan.FromMinutes(minutesLeft))
            };
            var messageViewModel = new MessageViewModel(message);
            bool result = _roomViewModel.RemoveMessageCommand.CanExecute(messageViewModel);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Should_RemoveExistingMessage()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message1 = new Message
            {
                Id = "123456",
                Text = "A message",
                User = new User
                {
                    Id = "53307734c3599d1de448e192"
                },
                SentDate = _mainViewModel.CurrentDateTime.Subtract(TimeSpan.FromMinutes(1))
            };
            var message1ViewModel = new MessageViewModel(message1);

            var message2 = new Message
            {
                Id = "654321",
                Text = "Another message",
                User = new User
                {
                    Id = "44444444444444444"
                },
                SentDate = _mainViewModel.CurrentDateTime.Subtract(TimeSpan.FromMinutes(1))
            };
            var message2ViewModel = new MessageViewModel(message2);

            _roomViewModel.Messages.Add(message1ViewModel);
            _roomViewModel.Messages.Add(message2ViewModel);

            _roomViewModel.RemoveMessageCommand.Execute(message1ViewModel);

            // Assert
            Assert.Equal(1, _gitterApiService.UpdatedMessages);
            Assert.Equal(string.Empty, message1.Text);
            Assert.Equal(1, _telemetryService.EventsTracked);
            Assert.Equal(1, _roomViewModel.Messages.Count);
        }

        [Fact]
        public void Should_CopyMessage()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "123456",
                Text = "A message",
                User = new User
                {
                    Id = "53307734c3599d1de448e192"
                },
                SentDate = _mainViewModel.CurrentDateTime.Subtract(TimeSpan.FromMinutes(1))
            };
            var messageViewModel = new MessageViewModel(message);

            _roomViewModel.CopyMessageCommand.Execute(messageViewModel);

            // Assert
            Assert.Equal("A message ", _roomViewModel.TextMessage);
            Assert.Equal(1, _telemetryService.EventsTracked);
        }

        [Fact]
        public async Task ReceivingMessageFromApi_Should_ShowMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = true,
                User = new User
                {
                    Id = "abcdef",
                    Username = "Odonno"
                }
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(1, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task ReceivingOurOwnMessageFromApi_Should_NotShowMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = true,
                User = await _gitterApiService.GetCurrentUserAsync()
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task ReceivingMessageAlreadyReadFromApi_Should_NotShowMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = false,
                User = new User
                {
                    Id = "abcdef",
                    Username = "Odonno"
                }
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(14, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task ReceivingMessageFromApiOnDisabledRoom_Should_NotShowMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14,
                DisabledNotifications = true
            };

            TestInitialize(room);

            // Act
            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = true,
                User = new User
                {
                    Id = "abcdef",
                    Username = "Odonno"
                }
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task ClosingStream_Should_RemoveMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.CloseRealtimeStream();

            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = true,
                User = new User
                {
                    Id = "abcdef",
                    Username = "Odonno"
                }
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(14, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task ReopeningStream_Should_EnableMessageNotification()
        {
            // Arrange
            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            TestInitialize(room);

            // Act
            _roomViewModel.CloseRealtimeStream();
            _roomViewModel.OpenRealtimeStream();

            var message = new Message
            {
                Id = "a1d4gv",
                UnreadByCurrent = true,
                User = new User
                {
                    Id = "abcdef",
                    Username = "Odonno"
                }
            };
            _gitterApiService.StreamingMessages.OnNext(message);

            await Task.Delay(100);

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(1, _localNotificationService.NotificationsSent);
        }

        #endregion
    }
}
