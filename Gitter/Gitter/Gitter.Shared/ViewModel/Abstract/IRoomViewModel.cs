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
        int UnreadMessagesCount { get; }

        ICommand SendMessageCommand { get; }
        ICommand SendMessageWithParamCommand { get; }
        ICommand RemoveMessageCommand { get; }
        ICommand RespondToCommand { get; }
        ICommand RefreshCommand { get; }

        void RefreshUnreadCount();
    }
}
