using Gitter.Services.Abstract;
using System;

namespace Gitter.UnitTests.Fakes
{
    public class FakeTelemetryService : ITelemetryService
    {
        #region Fake Properties

        public int ExceptionsTracked { get; private set; }

        #endregion


        #region Methods

        public void Initialize()
        {
            ExceptionsTracked = 0;
        }

        public void TrackException(Exception ex)
        {
            ExceptionsTracked++;
        }

        #endregion
    }
}
