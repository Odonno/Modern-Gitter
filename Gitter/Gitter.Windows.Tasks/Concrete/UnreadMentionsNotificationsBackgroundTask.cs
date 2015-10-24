using Gitter.Configuration;
using GitterSharp.Model;
using System.Threading.Tasks;

namespace Gitter.Windows.Tasks
{
    public sealed class UnreadMentionsNotificationsBackgroundTask : NotificationsBackgroundTask
    {
        protected override async Task CreateNotificationAsync(Room room)
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
                    _localNotificationService.SendNotification(room.Name, notificationContent, id, room.Name);
                    _applicationStorageService.Save(id, room.UnreadMentions);
                }
            }
        }
    }
}
