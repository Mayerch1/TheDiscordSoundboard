using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using TheDiscordSoundboard.Models.Bot;

namespace TheDiscordSoundboard.Models
{
    public class Buttons
    {

        public long id { get; set; }

        public long position { get; set; }

        public string nick_name { get; set; }

        public bool is_earrape { get; set; }

        public bool is_loop { get; set; }

        // foreign key/entry
        public long? track_id { get; set; }
        public virtual TrackData track { get; set; }
    }
}
