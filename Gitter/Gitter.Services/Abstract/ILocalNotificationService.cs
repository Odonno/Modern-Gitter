using System.Threading.Tasks;

namespace Gitter.Services.Abstract
{
    public interface ILocalNotificationService
    {
        void SendNotification(string title, string content, string id = null);

        Task ClearNotificationGroup(string id);
    }
}
