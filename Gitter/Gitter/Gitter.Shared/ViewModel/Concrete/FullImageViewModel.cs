using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class FullImageViewModel : ViewModelBase, IFullImageViewModel
    {
        private string _source;
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                RaisePropertyChanged();
            }
        }
    }
}
