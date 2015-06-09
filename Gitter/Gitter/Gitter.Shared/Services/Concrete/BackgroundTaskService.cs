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
        public Dictionary<string, string> Tasks
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"NotificationsBackgroundTask", "Gitter.Tasks"}
                };
            }
        }


        public async Task RegisterTasksAsync()
        {
            foreach (var kvTask in Tasks)
            {
                if (BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == kvTask.Key))
                    break;

                await RegisterTaskAsync(kvTask.Key, kvTask.Value);
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

                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                taskBuilder.AddCondition(new SystemCondition(SystemConditionType.UserNotPresent));

                taskBuilder.Register();
            }
        }
    }
}
