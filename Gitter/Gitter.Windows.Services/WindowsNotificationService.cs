using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.Services.Concrete
{
    public class WindowsNotificationService : BaseNotificationService
    {
        public override Task ClearNotificationGroupAsync(string group)
        {
            throw new NotImplementedException();
        }
    }
}
