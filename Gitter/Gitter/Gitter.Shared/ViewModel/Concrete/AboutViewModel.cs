using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        #region Properties

        public string ApplicationDescription
        {
            get
            {
                return "A Gitter client application for Windows Phone 8";
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return
                    $"{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}";
            }
        }

        public string ApplicationPublisher
        {
            get
            {
                return "David BOTTIAU";
            }
        }

        public string ApplicationTitle
        {
            get
            {
                return "Modern Gitter";
            }
        }

        #endregion
    }
}