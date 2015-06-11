using System.Collections.Generic;
using System.Reactive.Subjects;
using Gitter.ViewModel.Abstract;

namespace Gitter.Services.Abstract
{
    public interface IEventService
    {
        Subject<bool> RefreshRooms { get; }
        Subject<IEnumerable<IMessageViewModel>> NotifyUnreadMessages { get; }
    }
}
