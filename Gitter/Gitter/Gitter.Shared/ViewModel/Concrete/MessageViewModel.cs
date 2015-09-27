using System;
using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;
using GitterSharp.Model;

namespace Gitter.ViewModel.Concrete
{
    public sealed class MessageViewModel : ViewModelBase, IMessageViewModel
    {
        #region Properties

        public Message Message { get; private set; }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        private string _html;
        public string Html
        {
            get { return _html; }
            set
            {
                _html = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _sentDate;
        public DateTime SentDate
        {
            get { return _sentDate; }
            set
            {
                _sentDate = value;
                RaisePropertyChanged();
            }
        }

        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged();
            }
        }

        private bool _read;
        public bool Read
        {
            get { return _read; }
            set
            {
                _read = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #region Constructor

        public MessageViewModel(Message message)
        {
            // Properties
            Message = message;

            Id = message.Id;
            Text = message.Text;
            Html = message.Html;
            SentDate = message.SentDate;
            User = message.User;
            Read = !message.UnreadByCurrent;


            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.

            }
            else
            {
                // Code runs "for real"

            }
        }

        #endregion


        #region Methods

        public void UpdateMessage(string text)
        {
            Text = text;
            Message.Text = text;
        }

        public void ReadByCurrent()
        {
            Read = true;
            Message.UnreadByCurrent = !Read;
        }

        #endregion
    }
}
