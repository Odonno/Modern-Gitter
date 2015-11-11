using System;
using System.Collections.Generic;

namespace Gitter.Services.Abstract
{
    public interface ITelemetryService
    {
        void Initialize();
        void TrackException(Exception ex, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);
        void TrackEvent(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);
    }
}
