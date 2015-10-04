using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Gitter.Configuration;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class WindowsPhoneNotificationService : BaseNotificationService, ILocalNotificationService
    {
        public override void SendNotification(string title, string content, string id = null, string group = null)
        {
            var notification = this.CreateToastNotification(title, content, id);

            notification.Tag = NotificationConstants.Tag;
            notification.Group = group;

            this.toastNotifier.Show(notification);
        }

        public override async Task ClearNotificationGroupAsync(string group)
        {
            // So that action items are not cleared immediately when app is in the foreground, add a small delay before clearing them
            await Task.Delay(TimeSpan.FromSeconds(3));

            ToastNotificationManager.History.Remove(NotificationConstants.Tag, group);
        }
    }
}