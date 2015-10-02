using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GitterSharp.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        DateTime CurrentDateTime { get; }
        User CurrentUser { get; }
        ObservableCollection<IRoomViewModel> Rooms { get; }
        IRoomViewModel SelectedRoom { get; set; }
        ObservableCollection<IRoomViewModel> SearchedRooms { get; }
        string SearchedRoomText { get; set; }

        ICommand SelectRoomCommand { get; }
        ICommand ChatWithUsCommand { get; }
        ICommand RefreshCommand { get; }
        ICommand ToggleSearchCommand { get; }

        void SelectRoom(string roomName);
        void OpenRealtimeStreams();
        void CloseRealtimeStreams();
    }
}
