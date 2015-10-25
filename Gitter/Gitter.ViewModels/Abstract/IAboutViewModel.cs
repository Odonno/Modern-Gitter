using Gitter.DataObjects.Concrete;
using System.Collections.Generic;

namespace Gitter.ViewModel.Abstract
{
    public interface IAboutViewModel
    {
        string ApplicationVersion { get; }
        IEnumerable<Version> Versions { get; }
    }
}