using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Concrete;
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

        #endregion


        #region Methods

        [Fact]
        public void CreateSimpleMain_Should_SetDefaultProperties()
        {
            // Arrange
            _gitterApiService = new FakeGitterApiServiceWithResult();
            _localNotificationService = new FakeLocalNotificationService();
            _applicationStorageService = new FakeApplicationStorageService();
            _progressIndicatorService = new FakeProgressIndicatorService();
            _passwordStorageService = new FakePasswordStorageService();
            _eventService = new EventService();
            _telemetryService = new FakeTelemetryService();
            _navigationService = new FakeNavigationService();

            // Act
            var mainViewModel = new MainViewModel(
                _gitterApiService,
                _localNotificationService,
                _applicationStorageService,
                _progressIndicatorService,
                _passwordStorageService,
                _eventService,
                _telemetryService,
                _navigationService);

            // Assert
            Assert.Equal(DateTime.Now.ToString(), mainViewModel.CurrentDateTime.ToString());
            Assert.Null(mainViewModel.CurrentUser);
        }

        #endregion
    }
}
