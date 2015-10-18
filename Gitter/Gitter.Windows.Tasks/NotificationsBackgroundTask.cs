using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Services;
using GitterSharp.Model;
using Gitter.Configuration;
using System.Collections.Generic;

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
                _gitterApiService.SetToken(token);

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

        private async Task CreateUnreadMentionsNotificationAsync(Room room)
        {
            // Retrieve id of messages that contains a mention
            string userId = _applicationStorageService.Retrieve(StorageConstants.UserId) as string;
            var unreadItems = await _gitterApiService.RetrieveUnreadChatMessagesAsync(userId, room.Id);

            // Retrieve each message that contains mentions
            foreach (string mention in unreadItems.Mentions)
            {
                var message = await _gitterApiService.GetSingleRoomMessageAsync(room.Id, mention);

                string id = $"{room.Name}_mention_{message.Id}";
                if (!_applicationStorageService.Exists(id))
                {
                    // Show notifications (toast notifications)
                    string notificationContent = $"{message.User.Username} mentioned you";
                    _localNotificationService.SendNotification(room.Name, notificationContent, id);
                    _applicationStorageService.Save(id, room.UnreadMentions);
                }
            }
        }

        #endregion
    }
}
