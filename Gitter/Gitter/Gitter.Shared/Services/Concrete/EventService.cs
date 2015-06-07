using System.Reactive.Subjects;
using Gitter.Services.Abstract;

namespace Gitter.Services.Concrete
{
    public class EventService : IEventService
    {
        #region Properties

        public Subject<bool> RefreshRooms { get; private set; }

        #endregion


        #region Constructor

        public EventService()
        {
            RefreshRooms = new Subject<bool>();
        }

        #endregion
    }
}
