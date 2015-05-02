using System.Collections.ObjectModel;
using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        ObservableCollection<Room> Rooms { get; }

        ICommand SelectRoomCommand { get; }
    }
}
