using GitterSharp.Model;
using System.Threading.Tasks;

namespace Gitter.WindowsPhone.Tasks
{
    public sealed class UnreadItemsNotificationsBackgroundTask : NotificationsBackgroundTask
    {
        protected override async Task CreateNotificationAsync(Room room)
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
    }
}
