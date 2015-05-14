namespace Gitter.Model
{
    using Newtonsoft.Json;

    /// <summary>
    /// The send message.
    /// </summary>
    public class SendMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessage"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public SendMessage(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
