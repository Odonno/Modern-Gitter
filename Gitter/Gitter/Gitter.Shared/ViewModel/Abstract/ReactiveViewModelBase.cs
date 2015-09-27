using System.ComponentModel;
using Windows.ApplicationModel;
using ReactiveUI;

namespace Gitter.ViewModel.Abstract
{
    public abstract class ReactiveViewModelBase : ReactiveObject, INotifyPropertyChanged
    {
        private static bool? _isInDesignMode;
        public bool IsInDesignMode => IsInDesignModeStatic;

        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                    _isInDesignMode = DesignMode.DesignModeEnabled;

                return _isInDesignMode.Value;
            }
        }
    }
}
