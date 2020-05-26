using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models
{
    public class TrackData
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // if both are set, LocalFile is always preffered
        public string LocalFile { get; set; }
        public string Uri { get; set; }


        public string ImageUri { get; set; }
        public string Description { get; set; }

        public string Author { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }

        public int Duration { get; set; }
    }
}
