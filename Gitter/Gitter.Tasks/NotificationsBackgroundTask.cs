using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Services;
using GitterSharp.Model;

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
        private readonly IPasswordStorageService _passwordStorageService;
        private readonly IApplicationStorageService _applicationStorageService;

        #endregion


        #region Constructor

        public NotificationsBackgroundTask()
        {
            _localNotificationService = new LocalNotificationService();
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
                _gitterApiService.TryAuthenticate(token);

                // Retrieve rooms that user want notifications
                var notifyableRooms = (await _gitterApiService.GetRoomsAsync()).Where(room => !room.DisabledNotifications);

                // Add notifications for unread messages
                foreach (var room in notifyableRooms)
                {
                    // Show notifications (if possible)
                    CreateUnreadItemsNotification(room);
                    CreateUnreadMentionsNotification(room);
                }
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private void CreateUnreadItemsNotification(Room room)
        {
            string id = room.Name;

            // Detect if there is no new notification to launch (no unread messages)
            if (_applicationStorageService.Exists(id))
            {
                if (room.UnreadItems == 0)
                    _applicationStorageService.Remove(id); // Reset notification id for the future
                return;
            }

            // Show notifications (toast notifications)
            if (room.UnreadItems > 0)
            {
                string notificationContent = $"You have {room.UnreadItems} unread messages";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);

                _applicationStorageService.Save(id, room.UnreadItems);
            }
        }

        private void CreateUnreadMentionsNotification(Room room)
        {
            string id = $"{room.Name}_mention";

            // Detect if there is no new notification to launch (no unread messages)
            if (_applicationStorageService.Exists(id))
            {
                if (room.UnreadMentions == 0)
                    _applicationStorageService.Remove(id); // Reset notification id for the future
                return;
            }

            // Show notifications (toast notifications)
            if (room.UnreadMentions > 0)
            {
                // TODO : Retrieve mentions content to know who mentioned you
                string notificationContent = $"Someone mentioned you";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);

                _applicationStorageService.Save(id, room.UnreadMentions);
            }
        }

        #endregion
    }
}
