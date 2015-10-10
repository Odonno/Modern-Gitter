using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Gitter.Services.Abstract
{
    public abstract class BaseNotificationService : ILocalNotificationService
    {
        #region Fields

        protected ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();

        #endregion


        #region Methods

        public void SendNotification(string title, string content, string id = null, string group = null)
        {
            var notification = CreateToastNotification(title, content, id);
            ToastNotifier.Show(notification);
        }

        public abstract Task ClearNotificationGroupAsync(string group);

        protected virtual ToastNotification CreateToastNotification(string title, string content, string id = null, string group = null)
        {
            // Create notification form
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

            // Add launch parameter
            if (!string.IsNullOrWhiteSpace(id))
            {
                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\", \"id\":\"" + id + "\"}");
            }

            // Return the notification from the template
            return new ToastNotification(toastXml);
        }

        #endregion
    }
}