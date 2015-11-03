using System;
using Gitter.ViewModel.Concrete;
using Xunit;
using Gitter.Services.Abstract;
using GalaSoft.MvvmLight.Views;
using System.Threading.Tasks;
using Gitter.UnitTests.Fakes;

namespace Gitter.UnitTests.ViewModels
{
    public class LoginTests
    {
        #region Fields

        private INavigationService _navigationService;
        private ISessionService _sessionService;
        private IPasswordStorageService _passwordStorageService;
        private ILocalNotificationService _localNotificationService;

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

        #endregion
    }
}
