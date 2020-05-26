using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models.Bot
{
    /// <summary>
    /// only to be stored in in-memory List<>
    /// </summary>
    public class BotTrackData
    {

        public ReplayMetadata Metadata { get; set; } = new ReplayMetadata();

        public TrackData Track { get; set; }
    }
}
