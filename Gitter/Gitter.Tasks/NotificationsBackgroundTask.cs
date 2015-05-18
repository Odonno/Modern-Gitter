using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Gitter.Tasks
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        #region Fields

        private BackgroundTaskDeferral _deferral;

        #endregion


        #region Methods

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            await Do();
        }

        private async Task Do()
        {
            try
            {
                // TODO : Detect unread chat messages

                // TODO : Update notifications for unread messages (add / update / remove)
            }
            finally
            {
                _deferral.Complete();
            }
        }

        #endregion
    }
}
