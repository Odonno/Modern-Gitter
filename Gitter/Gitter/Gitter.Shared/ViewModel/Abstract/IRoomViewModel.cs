using System.Windows.Input;
using Gitter.DataObjects.Concrete;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        bool IsLoaded { get; set; }
        Room Room { get; }
        MessagesIncrementalLoadingCollection Messages { get; }
        string TextMessage { get; set; }
        int UnreadMessagesCount { get; set; }

        ICommand SendMessageCommand { get; }
        ICommand SendMessageWithParamCommand { get; }
        ICommand RemoveMessageCommand { get; }
        ICommand CopyMessageCommand { get; }
        ICommand RespondToCommand { get; }
        ICommand TalkCommand { get; }
        ICommand RefreshCommand { get; }
    }
}
