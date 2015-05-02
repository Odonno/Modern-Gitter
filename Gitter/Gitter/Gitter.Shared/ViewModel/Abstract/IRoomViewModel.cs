using System.Collections.ObjectModel;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        Room Room { get; set; }
        ObservableCollection<Message> Messages { get; }
    }
}
