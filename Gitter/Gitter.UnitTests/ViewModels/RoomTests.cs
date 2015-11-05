using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Concrete;
using GitterSharp.Model;
using System;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class RoomTests
    {
        // TODO : Test property IsLoaded

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

            var gitterApiService = new FakeGitterApiServiceWithResult();
            var localNotificationService = new FakeLocalNotificationService();
            var progressIndicatorService = new FakeProgressIndicatorService();
            var eventService = new EventService();
            var applicationStorageService = new FakeApplicationStorageService();
            var passwordStorageService = new FakePasswordStorageService();
            var navigationService = new FakeNavigationService();

            var mainViewModel = new MainViewModel(
                gitterApiService,
                localNotificationService,
                applicationStorageService,
                progressIndicatorService,
                passwordStorageService,
                eventService,
                navigationService);

            var roomViewModel = new RoomViewModel(room,
                gitterApiService,
                localNotificationService,
                progressIndicatorService,
                eventService,
                mainViewModel);

            // Act

            // Assert
            Assert.Same(room, roomViewModel.Room);
            Assert.NotNull(roomViewModel.Messages);
            Assert.Equal(0, roomViewModel.Messages.Count);
            Assert.Equal(14, roomViewModel.UnreadMessagesCount);
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

            var gitterApiService = new FakeGitterApiServiceWithResult();
            var localNotificationService = new FakeLocalNotificationService();
            var progressIndicatorService = new FakeProgressIndicatorService();
            var eventService = new EventService();
            var applicationStorageService = new FakeApplicationStorageService();
            var passwordStorageService = new FakePasswordStorageService();
            var navigationService = new FakeNavigationService();

            var mainViewModel = new MainViewModel(
                gitterApiService,
                localNotificationService,
                applicationStorageService,
                progressIndicatorService,
                passwordStorageService,
                eventService,
                navigationService);

            var roomViewModel = new RoomViewModel(room,
                gitterApiService,
                localNotificationService,
                progressIndicatorService,
                eventService,
                mainViewModel);

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
            gitterApiService.StreamingMessages.OnNext(message);

            // Assert
            Assert.Equal(15, roomViewModel.UnreadMessagesCount);
            Assert.True(localNotificationService.NotificationSent);
        }

        [Fact]
        public void SendingMessageOurselfFromApi_Should_NotShowMessageNotification()
        {
            // TODO
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
    }
}
