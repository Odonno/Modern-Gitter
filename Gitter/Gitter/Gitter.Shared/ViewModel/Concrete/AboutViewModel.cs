using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;
using Windows.ApplicationModel;

namespace Gitter.ViewModel.Concrete
{
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        #region Properties

        public string ApplicationVersion
        {
            get
            {
                return $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
            }
        }

        #endregion
    }
}