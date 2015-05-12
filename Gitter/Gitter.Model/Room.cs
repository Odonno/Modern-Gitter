/* Room schema
 * https://developer.gitter.im/docs/rooms-resource
 *  id: Room ID.
 *  name: Room name.
 *  topic: Room topic. (default: GitHub repo description)
 *  uri: Room URI on Gitter.
 *  oneToOne: Indicates if the room is a one-to-one chat.
 *  users: List of users in the room.
 *  userCount: Count of users in the room.
 *  unreadItems: Number of unread messages for the current user.
 *  mentions: Number of unread mentions for the current user.
 *  lastAccessTime: Last time the current user accessed the room in ISO format.
 *  lurk: Indicates if the current user has disabled notifications.
 *  url: Path to the room on gitter.
 *  githubType: Type of the room.
 *  v: Room version. 
*/

namespace Gitter.Model
{
    /// <summary>
    /// The room.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets the github type.
        /// </summary>
        public string githubType { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the last access time.
        /// </summary>
        public string lastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lurk.
        /// </summary>
        public bool lurk { get; set; }

        /// <summary>
        /// Gets or sets the mentions.
        /// </summary>
        public int mentions { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether one to one.
        /// </summary>
        public bool oneToOne { get; set; }

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        public string security { get; set; }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        public string topic { get; set; }

        /// <summary>
        /// Gets or sets the unread items.
        /// </summary>
        public int unreadItems { get; set; }

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        public string uri { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User user { get; set; }

        /// <summary>
        /// Gets or sets the user count.
        /// </summary>
        public int? userCount { get; set; }

        /// <summary>
        /// Gets or sets the v.
        /// </summary>
        public int? v { get; set; }
    }
}