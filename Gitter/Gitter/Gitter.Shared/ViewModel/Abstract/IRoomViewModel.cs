using System.Threading.Tasks;
using Gitter.DataObjects.Concrete;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        Room Room { get; }
        MessagesIncrementalLoadingCollection Messages { get; }
        string TextMessage { get; set; }

        Task RefreshAsync();
    }
}
