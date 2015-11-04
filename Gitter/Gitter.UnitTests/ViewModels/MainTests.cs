using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Concrete;
using System;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class MainTests
    {
        #region Fields

        private FakeGitterApiService _gitterApiService;
        private FakeLocalNotificationService _localNotificationService;
        private FakeApplicationStorageService _applicationStorageService;
        private FakeProgressIndicatorService _progressIndicatorService;
        private FakePasswordStorageService _passwordStorageService;
        private IEventService _eventService;
        private FakeNavigationService _navigationService;

        #endregion


        #region Methods

        [Fact]
        public void CreateSimpleMain_Should_SetDefaultProperties()
        {
            // Arrange
             _gitterApiService = new FakeGitterApiService();
            _localNotificationService = new FakeLocalNotificationService();
            _applicationStorageService = new FakeApplicationStorageService();
            _progressIndicatorService = new FakeProgressIndicatorService();
            _passwordStorageService = new FakePasswordStorageService();
            _eventService = new EventService();
            _navigationService = new FakeNavigationService();

            var mainViewModel = new MainViewModel(
                _gitterApiService,
                _localNotificationService,
                _applicationStorageService,
                _progressIndicatorService,
                _passwordStorageService,
                _eventService,
                _navigationService);

            // Act

            // Assert
            Assert.Equal(DateTime.Now.ToString(), mainViewModel.CurrentDateTime.ToString());
            Assert.Equal(false, _localNotificationService.NotificationSent);
            Assert.Null(mainViewModel.CurrentUser);
        }

        #endregion
    }
}
