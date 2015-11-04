using Gitter.ViewModel.Concrete;
using Xunit;
using System.Threading.Tasks;
using Gitter.UnitTests.Fakes;
using Gitter.ViewModel.Abstract;
using Gitter.Services.Abstract;

namespace Gitter.UnitTests.ViewModels
{
    public class LoginTests
    {
        #region Fields

        private FakeNavigationService _navigationService;
        private ISessionService _sessionService;
        private FakePasswordStorageService _passwordStorageService;
        private FakeLocalNotificationService _localNotificationService;
        private FakeTelemetryService _telemetryService;

        private ILoginViewModel _loginViewModel;

        #endregion


        #region Initialize

        private void TestInitialize(ISessionService sessionService)
        {
            _navigationService = new FakeNavigationService();
            _sessionService = sessionService;
            _passwordStorageService = new FakePasswordStorageService();
            _localNotificationService = new FakeLocalNotificationService();
            _telemetryService = new FakeTelemetryService();

            _telemetryService.Initialize();

            _loginViewModel = new LoginViewModel(
                _navigationService,
                _sessionService,
                _passwordStorageService,
                _localNotificationService,
                _telemetryService);
        }

        #endregion


        #region Methods

        [Fact]
        public async Task LoginWithExistingStoredToken_Should_Success()
        {
            // Arrange
            var sessionService = new FakeSessionServiceWithException();
            TestInitialize(sessionService);

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
            var sessionService = new FakeSessionServiceWithException();
            TestInitialize(sessionService);

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            Assert.Equal(1, _telemetryService.ExceptionsTracked);
            Assert.True(_localNotificationService.NotificationSent);
        }

        [Fact]
        public async Task LoginWithoutAuthentication_Should_ShowError()
        {
            // Arrange
            var sessionService = new FakeSessionServiceWithResult();
            sessionService.Result = null;

            TestInitialize(sessionService);

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            Assert.Equal(0, _telemetryService.ExceptionsTracked);
            Assert.True(_localNotificationService.NotificationSent);
        }

        [Fact]
        public async Task LoginWithWrongAuthentication_Should_DoNothing()
        {
            // Arrange
            var sessionService = new FakeSessionServiceWithResult();
            sessionService.Result = false;

            TestInitialize(sessionService);

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            Assert.Equal(0, _telemetryService.ExceptionsTracked);
            Assert.False(_localNotificationService.NotificationSent);
            Assert.Null(_navigationService.CurrentPageKey);
        }

        [Fact]
        public async Task LoginWithCorrectAuthentication_Should_Success()
        {
            // Arrange
            var sessionService = new FakeSessionServiceWithResult();
            sessionService.Result = true;

            TestInitialize(sessionService);

            // Act
            await _loginViewModel.LoginAsync();

            // Assert
            Assert.Equal(0, _telemetryService.ExceptionsTracked);
            Assert.False(_localNotificationService.NotificationSent);
            Assert.Equal("Main", _navigationService.CurrentPageKey);
        }

        #endregion
    }
}
