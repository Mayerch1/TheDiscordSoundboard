using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using TheDiscordSoundboard.Models.Bot;

namespace TheDiscordSoundboard.Models
{
    public class Buttons
    {

        public long Id { get; set; }

        public long Position { get; set; }

        public string NickName { get; set; }

        public bool IsEarrape { get; set; }

        public bool IsLoop { get; set; }

        // foreign key/entry
        public long? TrackId { get; set; }
        public virtual TrackData Track { get; set; }
    }
}
