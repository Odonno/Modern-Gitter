namespace Gitter.Services.Abstract
{
    public interface IRatingService
    {
        int StartedCount { get; }
        bool ReviewedBefore { get; }
        string LastVersionStarted { get; }


        void AskForRating();
    }
}
