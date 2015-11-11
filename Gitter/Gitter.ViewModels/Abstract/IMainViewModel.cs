using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GitterSharp.Model;
using System.Threading.Tasks;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        DateTime CurrentDateTime { get; }
        User CurrentUser { get; set; }
        ObservableCollection<IRoomViewModel> Rooms { get; }
        IRoomViewModel SelectedRoom { get; set; }
        ObservableCollection<IRoomViewModel> SearchedRooms { get; }
        string SearchedRoomText { get; set; }

        ICommand SelectRoomCommand { get; }
        ICommand ChatWithUsCommand { get; }
        ICommand GoToAboutPageCommand { get; }
        ICommand RefreshCommand { get; }
        ICommand ToggleSearchCommand { get; }

        Task RefreshAsync();
        void SelectRoom(string roomName);
        void OpenRealtimeStreams();
        void CloseRealtimeStreams();
    }
}
