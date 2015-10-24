using Gitter.Configuration;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Model;
using GitterSharp.Services;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Gitter.WindowsPhone.Tasks
{
    public sealed class UnreadMentionsNotificationsBackgroundTask : IBackgroundTask
    {
        #region Fields

        private BackgroundTaskDeferral _deferral;

        #endregion


        #region Services

        private readonly ILocalNotificationService _localNotificationService;
        private readonly IGitterApiService _gitterApiService;
        private readonly IPasswordStorageService _passwordStorageService;
        private readonly IApplicationStorageService _applicationStorageService;

        #endregion


        #region Constructor

        public UnreadMentionsNotificationsBackgroundTask()
        {
            _localNotificationService = new WindowsPhoneNotificationService();
            _gitterApiService = new GitterApiService();
            _passwordStorageService = new PasswordStorageService();
            _applicationStorageService = new ApplicationStorageService();
        }

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
                // Retrieve token from local storage
                string token = _passwordStorageService.Retrieve("token");

                // You need to be authenticated first to get current notifications
                _gitterApiService.SetToken(token);

                // Retrieve rooms that user want notifications
                var notifyableRooms = (await _gitterApiService.GetRoomsAsync()).Where(room => !room.DisabledNotifications);

                // Add notifications for unread messages
                foreach (var room in notifyableRooms)
                {
                    // Show notifications (if possible)
                    await CreateNotificationAsync(room);
                }
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async Task CreateNotificationAsync(Room room)
        {
            // Retrieve id of messages that contains a mention
            string userId = _applicationStorageService.Retrieve(StorageConstants.UserId) as string;
            var unreadItems = await _gitterApiService.RetrieveUnreadChatMessagesAsync(userId, room.Id);

            // Retrieve each message that contains mentions
            foreach (string mention in unreadItems.Mentions)
            {
                var message = await _gitterApiService.GetSingleRoomMessageAsync(room.Id, mention);

                string id = $"{room.Name}_mention_{message.Id}";
                if (!_applicationStorageService.Exists(id))
                {
                    // Show notifications (toast notifications)
                    string notificationContent = $"{message.User.Username} mentioned you";
                    _localNotificationService.SendNotification(room.Name, notificationContent, id);
                    _applicationStorageService.Save(id, room.UnreadMentions);
                }
            }
        }

        #endregion
    }
}
