using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models.Bot
{
    /// <summary>
    /// Contains volatile data about state of bot
    /// Everything in here is lost on application restart
    /// </summary>
    public class BotState
    {
        /// <summary>
        /// currently on play (or on pause)
        /// </summary>
        public BotTrackData Playing { get; set; }


        /// <summary>
        /// the songs to be played
        /// </summary>
        public List<BotTrackData> Queue { get; set; } = new List<BotTrackData>();
    }
}
