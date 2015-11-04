using Gitter.Services.Abstract;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Gitter.Services.Concrete
{
    public class TelemetryService : ITelemetryService
    {
        #region Properties

        public TelemetryClient Client { get; set; }

        #endregion


        #region Methods

        public void Initialize()
        {
#if DEBUG
            Client = new TelemetryClient(new TelemetryConfiguration { DisableTelemetry = true });
#else
            Client = new TelemetryClient();
#endif
        }

        #endregion
    }
}
