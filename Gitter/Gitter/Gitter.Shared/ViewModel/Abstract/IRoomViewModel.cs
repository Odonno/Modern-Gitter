using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        Room Room { get; set; }
        ObservableCollection<Message> Messages { get; }
        string TextMessage { get; set; }

        Task RefreshAsync();
    }
}
