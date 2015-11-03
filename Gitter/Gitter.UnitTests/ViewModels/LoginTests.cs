using System;
using Gitter.ViewModel.Concrete;
using Xunit;
using System.Threading.Tasks;
using Gitter.UnitTests.Fakes;

namespace Gitter.UnitTests.ViewModels
{
    public class LoginTests
    {
        #region Fields

        private FakeNavigationService _navigationService;
        private FakeSessionService _sessionService;
        private FakePasswordStorageService _passwordStorageService;
        private FakeLocalNotificationService _localNotificationService;

        #endregion


        #region Methods

        [Fact]
        public async Task LoginWithExistingStoredToken_Should_Success()
        {
            // Arrange
            _navigationService = new FakeNavigationService();
            _sessionService = new FakeSessionService();
            _passwordStorageService = new FakePasswordStorageService();
            _localNotificationService = new FakeLocalNotificationService();

            _passwordStorageService.Content = "123456";

            var loginViewModel = new LoginViewModel(
                _navigationService,
                _sessionService,
                _passwordStorageService,
                _localNotificationService);

            // Act
            await loginViewModel.LoginAsync();

            // Assert
            Assert.Equal("Main", _navigationService.CurrentPageKey);
        }

        [Fact]
        public async Task LoginWithFailedAuthentication_Should_ShowError()
        {
            // Arrange
            _navigationService = new FakeNavigationService();
            _sessionService = new FakeSessionService();
            _passwordStorageService = new FakePasswordStorageService();
            _localNotificationService = new FakeLocalNotificationService();

            var loginViewModel = new LoginViewModel(
                _navigationService,
                _sessionService,
                _passwordStorageService,
                _localNotificationService);

            // Act
            await loginViewModel.LoginAsync();

            // Assert
            Assert.Equal(true, _localNotificationService.NotificationSent);
        }

        #endregion
    }
}
