using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.API.Services.Abstract;
using Gitter.API.Services.Concrete;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;

namespace Gitter.Tasks
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        #region Fields

        private BackgroundTaskDeferral _deferral;
        
        #endregion


        #region Services

        private readonly ILocalNotificationService _localNotificationService;
        private readonly IGitterApiService _gitterApiService;

        #endregion


        #region Constructor

        public NotificationsBackgroundTask()
        {
            _localNotificationService = new LocalNotificationService();
            _gitterApiService = new GitterApiService(new ApplicationStorageService());
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
                // you need to be authenticated first to get current notifications
                _gitterApiService.TryAuthenticate();

                // Detect unread chat messages (rooms with unread messages)
                var roomsWithUnreadMessages = (await _gitterApiService.GetRoomsAsync())
                    .Where(room => !room.DisabledNotifications && room.UnreadItems > 0);

                // TODO : Update notifications for unread messages (add / update / remove)
                foreach (var room in roomsWithUnreadMessages)
                {
                    // show notifications (toast notifications)
                    string notificationContent = string.Format("You have {0} unread messages", room.UnreadItems);
                    _localNotificationService.SendNotification(room.Name, notificationContent);
                }
            }
            finally
            {
                _deferral.Complete();
            }
        }

        #endregion
    }
}
