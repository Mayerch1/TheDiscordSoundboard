using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models.Bot
{
    public class ReplayMetadata
    {
        public bool IsEarrape { get; set; }
        public bool IsLoop { get; set; }


        /// <summary>
        /// when this element is first in a list
        /// the currently playing item will be skipped
        /// -> this can lead to instant skipping an entire list
        /// -> if all titles are tagget ForceReplay
        /// -> should only be set for instantButtons etc.
        /// </summary>
        public bool ForceReplay { get; set; }
    }
}
