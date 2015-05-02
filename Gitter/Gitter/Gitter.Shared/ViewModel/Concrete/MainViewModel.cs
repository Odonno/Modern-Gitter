using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gitter.ViewModel.Abstract;

namespace Gitter.ViewModel.Concrete
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region Properties

        public IRoomsViewModel Rooms { get; private set; }

        private IRoomViewModel _selectedRoom;
        public IRoomViewModel SelectedRoom
        {
            get
            {
                return _selectedRoom;
            }
            set
            {
                _selectedRoom = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #region Commands
        
        public ICommand SelectRoomCommand { get; private set; }

        #endregion


        #region Constructor

        public MainViewModel()
        {
            // Commands
            SelectRoomCommand = new RelayCommand<IRoomViewModel>(SelectRoom);

            // ViewModels
            Rooms = ViewModelLocator.Rooms;


            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

                SelectedRoom = Rooms.Rooms.First();
            }
            else
            {
                // Code runs "for real"
            }
        }

        #endregion


        #region Command Methods

        private void SelectRoom(IRoomViewModel room)
        {
            SelectedRoom = room;
        }

        #endregion
    }
}