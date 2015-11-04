using System;

namespace Gitter.Services.Abstract
{
    public interface ITelemetryService
    {
        void Initialize();
        void TrackException(Exception ex);
    }
}
