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

            var roomViewModel = new RoomViewModel(room,
                gitterApiService,
                localNotificationService,
                progressIndicatorService,
                eventService);

            // Act

            // Assert
            Assert.Same(room, roomViewModel.Room);
            Assert.NotNull(roomViewModel.Messages);
            Assert.Equal(14, roomViewModel.UnreadMessagesCount);
        }
    }
}
