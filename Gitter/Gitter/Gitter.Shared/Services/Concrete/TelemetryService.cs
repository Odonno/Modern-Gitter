using System;
using Gitter.Services.Abstract;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

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

        public void TrackException(Exception ex)
        {
            _client.TrackException(ex);
        }

        #endregion
    }
}
