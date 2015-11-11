using Gitter.Services.Abstract;
using System;
using System.Collections.Generic;

namespace Gitter.UnitTests.Fakes
{
    public class FakeTelemetryService : ITelemetryService
    {
        #region Fake Properties

        public int ExceptionsTracked { get; private set; }
        public int EventsTracked { get; private set; }

        #endregion


        #region Methods

        public void Initialize()
        {
            ExceptionsTracked = 0;
            EventsTracked = 0;
        }

        public void TrackException(Exception ex, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            ExceptionsTracked++;
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            EventsTracked++;
        }

        #endregion
    }
}
