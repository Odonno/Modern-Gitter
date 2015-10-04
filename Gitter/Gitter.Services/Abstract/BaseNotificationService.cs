using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Gitter.Services.Abstract
{
    public abstract class BaseNotificationService
    {
        protected ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();

        public abstract void SendNotification(string title, string content, string id = null);

        public abstract Task ClearNotificationGroupAsync(string id);

        protected ToastNotification CreateToastNotification(string title, string content, string id = null)
        {
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

            // return the notification from the template before
            return new ToastNotification(toastXml);
        }
    }
}