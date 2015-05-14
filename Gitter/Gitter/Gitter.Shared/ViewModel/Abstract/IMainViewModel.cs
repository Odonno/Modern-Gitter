using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        User CurrentUser { get; }
        IRoomsViewModel Rooms { get; }
        IRoomViewModel SelectedRoom { get; }
        
        ICommand SelectRoomCommand { get; }
    }
}
