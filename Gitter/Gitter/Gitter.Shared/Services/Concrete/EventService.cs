using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Gitter.Services.Abstract;
using Gitter.ViewModel.Abstract;

namespace Gitter.Services.Concrete
{
    public class EventService : IEventService
    {
        #region Properties

        public Subject<bool> RefreshRooms { get; private set; }
        public Subject<IEnumerable<IMessageViewModel>> NotifyUnreadMessages { get; private set; }
        public Subject<Tuple<string, IMessageViewModel>> PushMessage { get; private set; }
        public Subject<IRoomViewModel> ReadRoom { get; private set; }

        #endregion


        #region Constructor

        public EventService()
        {
            RefreshRooms = new Subject<bool>();
            NotifyUnreadMessages = new Subject<IEnumerable<IMessageViewModel>>();
            PushMessage = new Subject<Tuple<string, IMessageViewModel>>();
            ReadRoom = new Subject<IRoomViewModel>();
        }

        #endregion
    }
}
