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
            _navigationService = new FakeNavigationService();

            _mainViewModel = new MainViewModel(
                _gitterApiService,
                _localNotificationService,
                _applicationStorageService,
                _progressIndicatorService,
                _passwordStorageService,
                _eventService,
                _navigationService);

            _roomViewModel = new RoomViewModel(room,
                _gitterApiService,
                _localNotificationService,
                _progressIndicatorService,
                _eventService,
                _mainViewModel);

            _localNotificationService.Reset();
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

        [Fact]
        public void SendingMessageFromApi_Should_ShowMessageNotification()
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

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(1, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public async Task SendingMessageOurselfFromApi_Should_NotShowMessageNotification()
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

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public void SendingMessageAlreadyReadFromApi_Should_NotShowMessageNotification()
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

            // Assert
            Assert.Equal(14, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public void SendingMessageFromApiOnDisabledRoom_Should_NotShowMessageNotification()
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

            // Assert
            Assert.Equal(15, _roomViewModel.UnreadMessagesCount);
            Assert.Equal(0, _localNotificationService.NotificationsSent);
        }

        [Fact]
        public void ClosingStream_Should_RemoveMessageNotification()
        {
            // TODO
        }

        [Fact]
        public void ReopeningStream_Should_EnableMessageNotification()
        {
            // TODO
        }

        #endregion
    }
}
