using System.Threading.Tasks;

namespace Gitter.Services.Abstract
{
    public interface ILocalNotificationService
    {
        void SendNotification(string title, string content, string id = null, string group = null);

        Task ClearNotificationGroupAsync(string group);
    }
}
