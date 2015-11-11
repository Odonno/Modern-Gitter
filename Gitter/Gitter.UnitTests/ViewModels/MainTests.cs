using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Concrete;
using GitterSharp.Model;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class MainTests
    {
        #region Fields

        private FakeGitterApiServiceWithResult _gitterApiService;
        private FakeLocalNotificationService _localNotificationService;
        private FakeApplicationStorageService _applicationStorageService;
        private FakeProgressIndicatorService _progressIndicatorService;
        private FakePasswordStorageService _passwordStorageService;
        private IEventService _eventService;
        private FakeTelemetryService _telemetryService;
        private FakeNavigationService _navigationService;

        private MainViewModel _mainViewModel;

        #endregion


        #region Initialize

        public void TestInitialize()
        {
            _gitterApiService = new FakeGitterApiServiceWithResult();
            _localNotificationService = new FakeLocalNotificationService();
            _applicationStorageService = new FakeApplicationStorageService();
            _progressIndicatorService = new FakeProgressIndicatorService();
            _passwordStorageService = new FakePasswordStorageService();
            _eventService = new EventService();
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
        }

        public RoomViewModel CreateRoomViewModel(Room room)
        {
            return new RoomViewModel(room,
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
        public void CreateSimpleMain_Should_SetDefaultProperties()
        {
            // Arrange
            TestInitialize();

            // Act
            // Assert
            Assert.Equal(DateTime.Now.ToString(), _mainViewModel.CurrentDateTime.ToString());
            Assert.Null(_mainViewModel.CurrentUser);
        }

        [Fact]
        public void SelectingRoomForTheFirstTime_Should_LoadIt()
        {
            // Arrange
            TestInitialize();

            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            var roomViewModel = CreateRoomViewModel(room);

            // Assert (before)
            Assert.False(roomViewModel.IsLoaded);

            // Act
            _mainViewModel.SelectRoomCommand.Execute(roomViewModel);

            // Assert (after)
            Assert.Same(roomViewModel, _mainViewModel.SelectedRoom);
            Assert.Equal(1, _telemetryService.EventsTracked);
            Assert.True(roomViewModel.IsLoaded);
        }

        [Fact]
        public void SelectingSameRoomTheSecondTime_Should_NotLoadItAgain()
        {
            // Arrange
            TestInitialize();

            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };

            var roomViewModel = CreateRoomViewModel(room);
            _mainViewModel.SelectRoomCommand.Execute(roomViewModel);

            // Assert (before)
            Assert.True(roomViewModel.IsLoaded);

            // Act
            _mainViewModel.SelectRoomCommand.Execute(roomViewModel);

            // Assert (after)
            Assert.Same(roomViewModel, _mainViewModel.SelectedRoom);
            Assert.Equal(2, _telemetryService.EventsTracked);
            Assert.True(roomViewModel.IsLoaded);
        }

        [Fact]
        public void SelectingRoomAndThenUnselectIt_Should_UnselectRoom()
        {
            // Arrange
            TestInitialize();

            var room = new Room
            {
                Id = "123456",
                Name = "Room",
                UnreadItems = 14
            };
            var roomViewModel = CreateRoomViewModel(room);

            _mainViewModel.SelectRoomCommand.Execute(roomViewModel);

            // Act
            _mainViewModel.SelectRoomCommand.Execute(null);

            // Assert
            Assert.Null(_mainViewModel.SelectedRoom);
            Assert.Equal(1, _telemetryService.EventsTracked);
        }

        [Fact]
        public void NoCurrentUser_Should_NotEnableChatWithUs()
        {
            // Arrange
            TestInitialize();

            // Act
            bool result = _mainViewModel.ChatWithUsCommand.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistingCurrentUser_Should_EnableChatWithUs()
        {
            // Arrange
            TestInitialize();

            // Act
            _mainViewModel.CurrentUser = await _gitterApiService.GetCurrentUserAsync();
            bool result = _mainViewModel.ChatWithUsCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ChatWithUsOnNonAlreadyJoinedRoom_Should_SelectRoom()
        {
            // Arrange
            TestInitialize();
            _mainViewModel.CurrentUser = await _gitterApiService.GetCurrentUserAsync();

            // Act
            _mainViewModel.ChatWithUsCommand.Execute(null);
            await Task.Delay(100);

            // Assert
            Assert.Equal(1, _gitterApiService.JoinedRooms);
            Assert.Equal(2, _telemetryService.EventsTracked);
            Assert.NotNull(_mainViewModel.SelectedRoom);
            Assert.Equal("Odonno/Modern-Gitter", _mainViewModel.SelectedRoom.Room.Name);
        }

        [Fact]
        public async Task ChatWithUsOnAlreadyJoinedRoom_Should_SelectRoom()
        {
            // Arrange
            TestInitialize();
            _mainViewModel.CurrentUser = await _gitterApiService.GetCurrentUserAsync();

            var room = new Room
            {
                Id = "1",
                Name = "Odonno/Modern-Gitter"
            };
            var roomViewModel = CreateRoomViewModel(room);
            _mainViewModel.Rooms.Add(roomViewModel);

            // Act
            _mainViewModel.ChatWithUsCommand.Execute(null);
            await Task.Delay(100);

            // Assert
            Assert.Equal(0, _gitterApiService.JoinedRooms);
            Assert.Equal(2, _telemetryService.EventsTracked);
            Assert.NotNull(_mainViewModel.SelectedRoom);
            Assert.Equal("Odonno/Modern-Gitter", _mainViewModel.SelectedRoom.Room.Name);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, null)]
        [InlineData(true, "Mo")]
        [InlineData(false, "Mo")]
        public void ToggleSearch_Should_ResetSearchedText(bool toggleValue, string searchedValue)
        {
            // Arrange
            TestInitialize();

            // Act
            _mainViewModel.SearchedRoomText = searchedValue;
            _mainViewModel.ToggleSearchCommand.Execute(toggleValue);

            // Assert
            Assert.Equal(string.Empty, _mainViewModel.SearchedRoomText);
        }

        [Fact]
        public async Task Refreshing_Should_SetCurrentUser()
        {
            // Arrange
            TestInitialize();

            // Assert (before)
            Assert.Null(_mainViewModel.CurrentUser);

            // Act
            await _mainViewModel.RefreshAsync();

            // Assert (after)
            Assert.False(_mainViewModel.IsRefreshing);
            Assert.NotNull(_mainViewModel.CurrentUser);
            Assert.Equal("malditogeek", _mainViewModel.CurrentUser.Username);
        }

        #endregion
    }
}
