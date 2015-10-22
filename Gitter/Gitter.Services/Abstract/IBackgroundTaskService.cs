using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gitter.Services.Abstract
{
    public interface IBackgroundTaskService
    {
        Dictionary<string, string> Tasks { get; }

        Task RegisterTasksAsync();
        void UnregisterTasks(params string[] taskNames);
    }
}
