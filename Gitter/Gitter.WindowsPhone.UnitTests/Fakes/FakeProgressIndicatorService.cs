using Gitter.Services.Abstract;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Gitter.UnitTests.Fakes
{
    public class FakeProgressIndicatorService : IProgressIndicatorService
    {
        #region Properties

        public StatusBarProgressIndicator ProgressIndicator { get; set; }

        #endregion


        #region Fake Properties

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
