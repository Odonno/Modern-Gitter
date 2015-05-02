using System.Collections.ObjectModel;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomsViewModel
    {
        ObservableCollection<IRoomViewModel> Rooms { get; }
    }
}
