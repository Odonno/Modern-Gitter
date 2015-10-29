using System.Collections.Generic;
using System.Windows.Input;
using Gitter.DataObjects.Concrete;

namespace Gitter.ViewModel.Abstract
{
    public interface IAboutViewModel
    {
        string ApplicationVersion { get; }
        IEnumerable<Version> Versions { get; }
        IEnumerable<Collaborator> Collaborators { get; }

        ICommand ViewProfileCommand { get; }
    }
}