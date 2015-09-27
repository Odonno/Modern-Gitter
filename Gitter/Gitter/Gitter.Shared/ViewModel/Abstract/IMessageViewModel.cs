using System;
using GitterSharp.Model;

namespace Gitter.ViewModel.Abstract
{
    public interface IMessageViewModel
    {
        Message Message { get; }
        string Id { get; }
        string Text { get; }
        DateTime SentDate { get; }
        User User { get; }
        bool Read { get; }


        void UpdateMessage(string text);
        void ReadByCurrent();
    }
}
