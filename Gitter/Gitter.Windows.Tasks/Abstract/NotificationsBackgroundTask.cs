using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Services;
using GitterSharp.Model;

namespace Gitter.Windows.Tasks
{
    public abstract class NotificationsBackgroundTask : IBackgroundTask
    {
        #region Fields

        private BackgroundTaskDeferral _deferral;

        #endregion


        #region Services

        protected readonly ILocalNotificationService _localNotificationService;
        protected readonly IGitterApiService _gitterApiService;
        protected readonly IPasswordStorageService _passwordStorageService;
        protected readonly IApplicationStorageService _applicationStorageService;

        #endregion


        #region Constructor

        public NotificationsBackgroundTask()
        {
            _localNotificationService = new WindowsNotificationService();
            _gitterApiService = new GitterApiService();
            _passwordStorageService = new PasswordStorageService();
            _applicationStorageService = new ApplicationStorageService();
        }

        #endregion


        #region Methods

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            await Do();
        }

        private async Task Do()
        {
            try
            {
                // Retrieve token from local storage
                string token = _passwordStorageService.Retrieve("token");

                // You need to be authenticated first to get current notifications
                _gitterApiService.SetToken(token);

                // Retrieve rooms that user want notifications
                var notifyableRooms = (await _gitterApiService.GetRoomsAsync()).Where(room => !room.DisabledNotifications);

                // Add notifications for unread messages
                foreach (var room in notifyableRooms)
                {
                    // Show notifications (if possible)
                    await CreateNotificationAsync(room);
                }
            }
            finally
            {
                _deferral.Complete();
            }
        }

        protected abstract Task CreateNotificationAsync(Room room);

        #endregion
    }
}
