using System;
using Gitter.Services.Abstract;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;

namespace Gitter.Services.Concrete
{
    public class TelemetryService : ITelemetryService
    {
        #region Fields

        private TelemetryClient _client { get; set; }

        #endregion


        #region Methods

        public void Initialize()
        {
#if DEBUG
            _client = new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true });
#else
            _client = new TelemetryClient();
#endif
        }

        public void TrackException(Exception ex, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            _client.TrackException(ex, properties, metrics);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            _client.TrackEvent(eventName, properties, metrics);
        }

        #endregion
    }
}
