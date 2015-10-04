using Gitter.Services.Abstract;
using System.Threading.Tasks;

namespace Gitter.Services.Concrete
{
    public class LocalNotificationService : BaseNotificationService, ILocalNotificationService
    {
        public override void SendNotification(string title, string content, string id = null, string group = null)
        {
            var notification = this.CreateToastNotification(title, content, id);

            this.toastNotifier.Show(notification);
        }

        public override Task ClearNotificationGroupAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
