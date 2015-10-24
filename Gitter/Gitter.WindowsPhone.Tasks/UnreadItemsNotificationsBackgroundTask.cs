using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Model;
using GitterSharp.Services;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Gitter.WindowsPhone.Tasks
{
    public sealed class UnreadItemsNotificationsBackgroundTask : IBackgroundTask
    {
        #region Fields

        private BackgroundTaskDeferral _deferral;

        #endregion


        #region Services

        private readonly ILocalNotificationService _localNotificationService;
        private readonly IGitterApiService _gitterApiService;
        private readonly IPasswordStorageService _passwordStorageService;
        private readonly IApplicationStorageService _applicationStorageService;

        #endregion


        #region Constructor

        public UnreadItemsNotificationsBackgroundTask()
        {
            _localNotificationService = new WindowsPhoneNotificationService();
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

        private async Task CreateNotificationAsync(Room room)
        {
            string id = room.Name;

            // Detect if there is no new notification to launch (no unread messages)
            if (_applicationStorageService.Exists(id))
            {
                // Reset notification id for the future
                if (room.UnreadItems == 0)
                    _applicationStorageService.Remove(id);

                return;
            }

            if (room.UnreadItems > 0)
            {
                // Show notifications (toast notifications)
                string notificationContent = $"You have {room.UnreadItems} unread messages";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);
                _applicationStorageService.Save(id, room.UnreadItems);
            }
        }

        #endregion
    }
}
