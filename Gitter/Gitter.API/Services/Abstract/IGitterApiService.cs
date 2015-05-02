namespace Gitter.API.Services.Abstract
{
    public interface IGitterApiService
    {
        #region Authentication

        string AccessToken { get; }
        void TryAuthenticate(string token = null);

        #endregion
    }
}
