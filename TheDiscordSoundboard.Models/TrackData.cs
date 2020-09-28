using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models
{
    public class TrackData
    {
        public long id { get; set; }
        public string name { get; set; }

        // if both are set, LocalFile is always preffered
        public string local_file { get; set; }
        public string uri { get; set; }


        public string image_uri { get; set; }
        public string description { get; set; }

        public string author { get; set; }
        public string album { get; set; }
        public string genre { get; set; }

        public int duration { get; set; }
    }
}
