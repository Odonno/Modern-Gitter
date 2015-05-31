using System;
using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        int CurrentSectionIndex { get; set; }
        DateTime CurrentDateTime { get; }
        User CurrentUser { get; }
        IRoomsViewModel Rooms { get; }
        IRoomViewModel SelectedRoom { get; }
        
        ICommand SelectRoomCommand { get; }
        ICommand ChatWithUsCommand { get; }

        void SelectRoom(string roomName);
    }
}
