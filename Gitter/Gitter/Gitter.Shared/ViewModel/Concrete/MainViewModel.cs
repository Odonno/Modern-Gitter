using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }
    }
}