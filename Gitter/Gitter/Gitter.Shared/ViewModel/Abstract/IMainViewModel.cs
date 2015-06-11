using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        DateTime CurrentDateTime { get; }
        User CurrentUser { get; }
        ObservableCollection<IRoomViewModel> Rooms { get; }
        IRoomViewModel SelectedRoom { get; }
        
        ICommand SelectRoomCommand { get; }
        ICommand ChatWithUsCommand { get; }
        ICommand RefreshCommand { get; }

        void SelectRoom(string roomName);
    }
}
