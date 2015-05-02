using System.Collections.ObjectModel;
using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        IRoomsViewModel Rooms { get; }
        IRoomViewModel SelectedRoom { get; }
        
        ICommand SelectRoomCommand { get; }
    }
}
