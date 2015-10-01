namespace Gitter.ViewModel.Abstract
{
    public interface IAboutViewModel
    {
        string ApplicationDescription { get; }

        string ApplicationVersion { get; }

        string ApplicationPublisher { get; }

        string ApplicationTitle { get; }
    }
}