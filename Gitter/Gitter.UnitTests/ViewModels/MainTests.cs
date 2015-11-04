using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Concrete;
using GitterSharp.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Gitter.UnitTests.ViewModels
{
    public class MainTests
    {
        #region Fields

        private IGitterApiService _gitterApiService;
        private FakeLocalNotificationService _localNotificationService;
        private FakeApplicationStorageService _applicationStorageService;
        private FakeProgressIndicatorService _progressIndicatorService;
        private FakePasswordStorageService _passwordStorageService;
        private IEventService _eventService;
        private FakeNavigationService _navigationService;

        #endregion


        #region Methods

        [Fact]
        public async Task CreateSimpleMainWithFailedApi_Should_SetDefaultProperties()
        {
            // Arrange
             _gitterApiService = new FakeGitterApiServiceWithException();
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
            await Task.Delay(1000);

            // Assert
            Assert.Equal(DateTime.Now.Subtract(TimeSpan.FromSeconds(1)).ToString(), mainViewModel.CurrentDateTime.ToString());
            Assert.True(_localNotificationService.NotificationSent);
            Assert.Null(mainViewModel.CurrentUser);
        }

        [Fact]
        public async Task CreateSimpleMainWithSuccessApi_Should_SetDefaultProperties()
        {
            // Arrange
            _gitterApiService = new FakeGitterApiServiceWithResult();
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
            await Task.Delay(1000);

            // Assert
            Assert.Equal(DateTime.Now.Subtract(TimeSpan.FromSeconds(1)).ToString(), mainViewModel.CurrentDateTime.ToString());
            Assert.False(_localNotificationService.NotificationSent);
            Assert.NotNull(mainViewModel.CurrentUser);
        }

        #endregion
    }
}
