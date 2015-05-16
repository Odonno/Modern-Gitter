using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void SendNotification(string title, string content)
        {
            // Send any notification (Tile, Toast)
            SendToastNotification(title, content);
        }


        private void SendToastNotification(string title, string content)
        {
            // get toast notifier
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();

            // create notification form
            XmlDocument toastXml;

            if (string.IsNullOrWhiteSpace(title))
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

                var toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(content));
            }
            else
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));
            }

            // create the notification from the template before
            var toastNotification = new ToastNotification(toastXml);

            // show notif
            toastNotifier.Show(toastNotification);
        }
    }
}
