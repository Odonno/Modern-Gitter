using System.Threading.Tasks;
using System.Windows.Input;
using Gitter.DataObjects.Concrete;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        Room Room { get; }
        MessagesIncrementalLoadingCollection Messages { get; }
        string TextMessage { get; set; }

        ICommand SendMessageCommand { get; }

        void Refresh();
    }
}
