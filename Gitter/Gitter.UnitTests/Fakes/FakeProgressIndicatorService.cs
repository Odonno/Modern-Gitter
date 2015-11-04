using Gitter.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Gitter.UnitTests.Fakes
{
    public class FakeProgressIndicatorService : IProgressIndicatorService
    {
        #region Properties

        public bool IsShowing { get; private set; }

        #endregion


        #region Methods

        public Task HideAsync()
        {
            return Task.Run(() => IsShowing = false);
        }

        public Task ShowAsync()
        {
            return Task.Run(() => IsShowing = true);
        }

        #endregion
    }
}
