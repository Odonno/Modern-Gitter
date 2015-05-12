/* Message schema
 * https://developer.gitter.im/docs/messages-resource
 *  id: ID of the message.
 *  text: Original message in plain-text/markdown.
 *  html: HTML formatted message.
 *  sent: ISO formated date of the message.
 *  editedAt: ISO formated date of the message if edited.
 *  fromUser: (User)[user-resource] that sent the message.
 *  unread: Boolean that indicats if the current user has read the messsage.
 *  readBy: Number of users that have read the message.
 *  urls: List of URLs present in the message.
 *  mentions: List of @Mentions in the message.
 *  issues: List of #Issues referenced in the message.
 *  meta: Metadata.
 *  v: Version. 
*/

namespace Gitter.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the edited at.
        /// </summary>
        public object editedAt { get; set; }

        /// <summary>
        /// Gets or sets the from user.
        /// </summary>
        public User fromUser { get; set; }

        /// <summary>
        /// Gets or sets the html.
        /// </summary>
        public string html { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the issues.
        /// </summary>
        public List<object> issues { get; set; }

        /// <summary>
        /// Gets or sets the mentions.
        /// </summary>
        public List<object> mentions { get; set; }

        /// <summary>
        /// Gets or sets the read by.
        /// </summary>
        public int readBy { get; set; }

        /// <summary>
        /// Gets or sets the sent.
        /// </summary>
        public string sent { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unread.
        /// </summary>
        public bool unread { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        public List<object> urls { get; set; }

        /// <summary>
        /// Gets or sets the v.
        /// </summary>
        public int v { get; set; }
    }
}