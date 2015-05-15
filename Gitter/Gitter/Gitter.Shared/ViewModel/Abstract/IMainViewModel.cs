using System;
using System.Windows.Input;
using Gitter.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMainViewModel
    {
        DateTime CurrentDateTime { get; }
        User CurrentUser { get; }
        IRoomsViewModel Rooms { get; }
        IRoomViewModel SelectedRoom { get; }
        
        ICommand SelectRoomCommand { get; }
    }
}
