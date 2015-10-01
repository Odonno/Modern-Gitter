﻿using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.Services.Abstract;
using Gitter.Services.Concrete;
using GitterSharp.Services;
using GitterSharp.Model;

namespace Gitter.Tasks
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
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

        public NotificationsBackgroundTask()
        {
            _localNotificationService = new LocalNotificationService();
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
                _gitterApiService.TryAuthenticate(token);

                // Retrieve rooms that user want notifications
                var notifyableRooms = (await _gitterApiService.GetRoomsAsync()).Where(room => !room.DisabledNotifications);

                // Add notifications for unread messages
                foreach (var room in notifyableRooms)
                {
                    if (_applicationStorageService.Exists(room.Name))
                    {
                        // Detect if there is no new notification to launch (no unread messages)
                        if (room.UnreadItems == 0)
                            _applicationStorageService.Remove(room.Name);

                        if (room.UnreadMentions == 0)
                            _applicationStorageService.Remove($"{room.Name}_mention");
                    }
                    else
                    {
                        // Show notifications (if possible)
                        CreateNotifications(room);
                    }
                }
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private void CreateNotifications(Room room)
        {
            if (room.UnreadItems > 0)
            {
                // Show notifications (toast notifications)
                string id = room.Name;
                string notificationContent = $"You have {room.UnreadItems} unread messages";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);

                _applicationStorageService.Save(id, room.UnreadItems);
            }

            if (room.UnreadMentions > 0)
            {
                // TODO : Retrieve mentions content to know who mentioned you
                string id = $"{room.Name}_mention";
                string notificationContent = $"Someone mentioned you";
                _localNotificationService.SendNotification(room.Name, notificationContent, id);

                _applicationStorageService.Save(id, room.UnreadMentions);
            }
        }

        #endregion
    }
}
