using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void SendNotification(string title, string content, string id = null)
        {
            // Send any notification (Tile, Toast)
            SendToastNotification(title, content, id);
        }


        private void SendToastNotification(string title, string content, string id = null)
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

            // add launch parameter
            if (!string.IsNullOrWhiteSpace(id))
            {
                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\", \"id\":\"" + id + "\"}");
            }

            // create the notification from the template before
            var toastNotification = new ToastNotification(toastXml);

            // show notif
            toastNotifier.Show(toastNotification);
        }
    }
}
