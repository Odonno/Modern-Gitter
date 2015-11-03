using Microsoft.ApplicationInsights;

namespace Gitter.Services.Abstract
{
    public interface ITelemetryService
    {
        TelemetryClient Client { get; set; }

        void Create();
    }
}
