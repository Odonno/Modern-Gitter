using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.UnitTests.Fakes
{
    public class FakeLocalNotificationService : ILocalNotificationService
    {
        #region Fake Properties

        public bool NotificationSent { get; private set; }

        #endregion


        #region Methods

        public Task ClearNotificationGroupAsync(string group)
        {
            throw new NotImplementedException();
        }

        public void SendNotification(string title, string content, string id = null, string group = null)
        {
            NotificationSent = true;
        }

        #endregion
    }
}
