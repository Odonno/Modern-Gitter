using System.Reactive.Subjects;

namespace Gitter.Services.Abstract
{
    public interface IEventService
    {
        Subject<bool> RefreshRooms { get; } 
    }
}
