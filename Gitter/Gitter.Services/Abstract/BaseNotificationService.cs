using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Gitter.Services.Abstract
{
    public abstract class BaseNotificationService
    {
        public void SendNotification(string title, string content, string id = null)
        {
            // Send any notification (Tile, Toast)
            SendToastNotification(title, content, id);
        }

        public async Task ClearNotificationGroup(string id)
        {
#if WINDOWS_PHONE_APP
            // So that action items are not cleared immediately when app is in the foreground, add a small delay before clearing them
            await Task.Delay(TimeSpan.FromSeconds(3));

            ToastNotificationManager.History.Remove("Modern Gitter", id);
#else
            throw new NotImplementedException();
#endif
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

#if WINDOWS_PHONE_APP
            toastNotification.Tag = "Modern Gitter";
            toastNotification.Group = id;
#endif

            // show notif
            toastNotifier.Show(toastNotification);
        }
    }
}