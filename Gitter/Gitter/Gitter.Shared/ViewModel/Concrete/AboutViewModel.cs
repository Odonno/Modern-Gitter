using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Gitter.ViewModel.Abstract;
using Windows.ApplicationModel;
using Gitter.DataObjects.Concrete;

namespace Gitter.ViewModel.Concrete
{
    public class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        #region Properties

        public string ApplicationVersion
        {
            get
            {
                return $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
            }
        }

        private readonly IEnumerable<Version> _versions = new List<Version>
        {
            new Version
            {
                Name = "Version 1.3 (soon)",
                Features = new List<string>
                {
                    "- fix bug on realtime messages",
                    "- fix bug on toast notification",
                    "- parse effectively all the HTML sent",
                    "- search rooms by name",
                    "- toast notifications (mention)",
                    "- about page"
                }
            },
            new Version
            {
                Name = "Version 1.2",
                Features = new List<string>
                {
                    "- copy / remove message",
                    "- toast notifications (unread messages)",
                    "- toast notifications (realtime messages)",
                    "- in-app notification as badge",
                    "- 'respond to' a user (tap on avatar)",
                    "- microphone enabled"
                }
            },
            new Version
            {
                Name = "Version 1.1",
                Features = new List<string>
                {
                    "- main page (v1)",
                    "- room page (v1)",
                    "- push a message",
                    "- add a link to this chat room"
                }
            }
        };
        public IEnumerable<Version> Versions { get { return _versions; } }

        #endregion
    }
}