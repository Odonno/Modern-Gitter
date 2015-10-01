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
                    await CreateUnreadMentionsNotificationAsync(room);
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

            // Show notifications (toast notifications)
            if (CanNotify(id, room.UnreadItems))
            {
                string notificationContent = $"You have {room.UnreadItems} unread messages";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);
                _applicationStorageService.Save(id, room.UnreadItems);
            }
        }

        private async Task CreateUnreadMentionsNotificationAsync(Room room)
        {
            string id = $"{room.Name}_mention";

            // Show notifications (toast notifications)
            if (CanNotify(id, room.UnreadMentions))
            {
                // TODO : Retrieve mentions content to know who mentioned you
                string userId = _applicationStorageService.Retrieve("User") as string;
                var unreadItems = await _gitterApiService.RetrieveUnreadChatMessagesAsync(userId, room.Id);

                string notificationContent = "Someone mentioned you";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);
                _applicationStorageService.Save(id, room.UnreadMentions);
            }
        }

        private bool CanNotify(string id, int itemCount)
        {
            // Detect if there is no new notification to launch (no unread messages)
            if (_applicationStorageService.Exists(id))
            {
                // Reset notification id for the future
                if (itemCount == 0)
                    _applicationStorageService.Remove(id);

                return false;
            }

            return itemCount > 0;
        }

        #endregion
    }
}
