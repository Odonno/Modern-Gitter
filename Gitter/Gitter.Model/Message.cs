using System;
using System.Collections.Generic;

namespace Gitter.Model
{
    public class Message
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? EditedDate { get; set; }
        public User User { get; set; }
        public bool ReadByCurrent { get; set; }
        public int ReadCount { get; set; }
        public IEnumerable<string> Urls { get; set; }
        public IEnumerable<Mention> Mentions { get; set; }
        public IEnumerable<Issue> Issues { get; set; }
        public int Version { get; set; }
    }
}
