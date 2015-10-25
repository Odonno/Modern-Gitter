using System.Windows.Input;
using GitterSharp.Model;
using Gitter.DataObjects.Abstract;

namespace Gitter.ViewModel.Abstract
{
    public interface IRoomViewModel
    {
        bool IsLoaded { get; set; }
        Room Room { get; }
        IncrementalLoadingCollection<IMessageViewModel> Messages { get; }
        string TextMessage { get; set; }
        int UnreadMessagesCount { get; set; }

        ICommand SendMessageCommand { get; }
        ICommand SendMessageWithParamCommand { get; }
        ICommand RemoveMessageCommand { get; }
        ICommand CopyMessageCommand { get; }
        ICommand RespondToCommand { get; }
        ICommand ViewProfileCommand { get; }
        ICommand TalkCommand { get; }
        ICommand RefreshCommand { get; }

        void OpenRealtimeStream();
        void CloseRealtimeStream();
    }
}
