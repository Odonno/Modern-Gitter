using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.Services
{
    public class WindowsNotificationService : BaseNotificationService
    {
        public override Task ClearNotificationGroupAsync(string group)
        {
            throw new NotImplementedException();
        }
    }
}
