namespace Gitter.Services.Abstract
{
    public interface ILocalNotificationService
    {
        void SendNotification(string title, string content, string id = null);
    }
}
