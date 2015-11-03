using System;
using Gitter.ViewModel.Concrete;
using Xunit;
using System.Threading.Tasks;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Abstract;

namespace Gitter.UnitTests.ViewModels
{
    public class LoginTests
    {
        #region Fields

        private FakeNavigationService _navigationService;
        private FakeSessionService _sessionService;
        private FakePasswordStorageService _passwordStorageService;
        private FakeLocalNotificationService _localNotificationService;

        private ILoginViewModel _loginViewModel;

        #endregion


        #region Initialize

        private void TestInitialize()
        {
            _navigationService = new FakeNavigationService();
            _sessionService = new FakeSessionService();
            _passwordStorageService = new FakePasswordStorageService();
            _localNotificationService = new FakeLocalNotificationService();

            _loginViewModel = new LoginViewModel(
                _navigationService,
                _sessionService,
                _passwordStorageService,
                _localNotificationService);
        }


        #endregion


        #region Methods

        [Fact]
        public async Task LoginWithExistingStoredToken_Should_Success()
        {
            // Arrange
            TestInitialize();

            _passwordStorageService.Content = "123456";

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            Assert.Equal("Main", _navigationService.CurrentPageKey);
        }

        [Fact]
        public async Task LoginWithFailedAuthentication_Should_ShowError()
        {
            // Arrange
            TestInitialize();

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            // TODO : Test the telemetry tracking
            Assert.Equal(true, _localNotificationService.NotificationSent);
        }

        #endregion
    }
}
