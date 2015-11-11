using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.UnitTests.Fakes
{
    public class FakeLocalNotificationService : ILocalNotificationService
    {
        #region Fake Properties

        public int NotificationsSent { get; private set; }

        #endregion


        #region Fake Methods

        public void Reset()
        {
            NotificationsSent = 0;
        }

        #endregion


        #region Methods

        public Task ClearNotificationGroupAsync(string group)
        {
            throw new NotImplementedException();
        }

        public void SendNotification(string title, string content, string id = null, string group = null)
        {
            NotificationsSent++;
        }

        #endregion
    }
}
