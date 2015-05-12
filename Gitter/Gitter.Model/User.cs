/* User schema
 * https://developer.gitter.im/docs/user-resource
 *  id: Gitter User ID.
 *  username: Gitter/GitHub username.
 *  displayName: Gitter/GitHub user real name.
 *  url: Path to the user on Gitter.
 *  avatarUrlSmall: User avatar URI (small).
 *  avatarUrlMedium: User avatar URI (medium).
*/

namespace Gitter.Model
{
    /// <summary>
    /// The user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the avatar url medium.
        /// </summary>
        public string avatarUrlMedium { get; set; }

        /// <summary>
        /// Gets or sets the avatar url small.
        /// </summary>
        public string avatarUrlSmall { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string username { get; set; }
    }
}