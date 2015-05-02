using System;
using System.Collections.Generic;

namespace Gitter.Model
{
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Url { get; set; }
        public bool OneToOne { get; set; }
        public IList<User> Users { get; set; }
        public int UserCount { get; set; }
        public int UnreadItems { get; set; }
        public int UnreadMentions { get; set; }
        public DateTime LastAccessTime { get; set; }
        public bool DisabledNotifications { get; set; }
        public RoomType Type { get; set; }
        public int Version { get; set; }
    }
}
