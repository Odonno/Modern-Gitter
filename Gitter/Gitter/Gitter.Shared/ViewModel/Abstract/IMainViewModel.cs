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

        ICommand SelectRoomCommand { get; }
        ICommand ChatWithUsCommand { get; }
        ICommand RefreshCommand { get; }

        void SelectRoom(string roomName);
    }
}
