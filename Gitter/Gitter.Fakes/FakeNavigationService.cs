using GalaSoft.MvvmLight.Views;
using System;

namespace Gitter.UnitTests.Fakes
{
    public class FakeNavigationService : INavigationService
    {
        #region Fake Properties

        private string _currentPageKey;
        public string CurrentPageKey { get { return _currentPageKey; } }

        #endregion


        #region Methods

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey)
        {
            _currentPageKey = pageKey;
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
