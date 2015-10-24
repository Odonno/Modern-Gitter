using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        #region Properties

        public Dictionary<string, string> Tasks
        {
            get
            {
#if WINDOWS_APP || WINDOWS_UWP
                string @namespace = "Gitter.Windows.Tasks";
#endif
#if WINDOWS_PHONE_APP
                string @namespace = "Gitter.WindowsPhone.Tasks";
#endif
                return new Dictionary<string, string>
                {
                    {"UnreadItemsNotificationsBackgroundTask", @namespace},
                    {"UnreadMentionsNotificationsBackgroundTask", @namespace}
                };
            }
        }

        #endregion


        #region Methods

        public async Task RegisterTasksAsync()
        {
            foreach (var kvTask in Tasks)
            {
                // Do not register again if this task already exists
                if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == kvTask.Key))
                    continue;

                // Register the task
                await RegisterTaskAsync(kvTask.Key, kvTask.Value);
            }
        }

        public void UnregisterTasks(params string[] taskNames)
        {
            foreach (string taskName in taskNames)
            {
                // Retrieve background tasks already running
                var existingBackgroundTasks = BackgroundTaskRegistration.AllTasks
                    .Where(task => task.Value.Name == taskName);

                // Unregister every task that run in background currently
                foreach (var existingBackgroundTask in existingBackgroundTasks)
                {
                    existingBackgroundTask.Value.Unregister(true);
                }
            }
        }

        private async Task RegisterTaskAsync(string taskName, string taskNamespace)
        {
            var requestAccess = await BackgroundExecutionManager.RequestAccessAsync();

            if (requestAccess == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity ||
                requestAccess == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
            {
                var taskBuilder = new BackgroundTaskBuilder
                {
                    Name = taskName,
                    TaskEntryPoint = string.Format("{0}.{1}", taskNamespace, taskName)
                };

                // Set the condition trigger that feels right for you
                taskBuilder.SetTrigger(new TimeTrigger(15, false));

                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserNotPresent));

                taskBuilder.Register();
            }
        }

        #endregion
    }
}
