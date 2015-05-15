namespace Gitter.API.Configuration
{
    public static class Constants
    {
        /// <summary>
        /// Base URL of the Gitter API
        /// </summary>
        public static string ApiBaseUrl = "https://api.gitter.im/";

        /// <summary>
        /// Base URL of the streaming Gitter API
        /// </summary>
        public static string StreamApiBaseUrl = "https://stream.gitter.im/";

        /// <summary>
        /// Version of the current Gitter API
        /// </summary>
        public static string ApiVersion = "v1/";

        /// <summary>
        /// OAuth key to authenticate user
        /// </summary>
        public static string OauthKey = "0f3fc414587a8d31a1514e005fa157168ad8efdb";

        /// <summary>
        /// OAuth secret to authenticate user
        /// </summary>
        public static string OauthSecret = "55c361ef1de79ffef1a49a1a0bff1a7a0140799c";

        /// <summary>
        /// Redirect URL when authenticate
        /// </summary>
        public static string RedirectUrl = "http://localhost";
    }
}
