using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models.config
{
    public class BotConfigDto
    {
        public BotConfigDto() { }
        public BotConfigDto(Config cfg)
        {
            this.Token = cfg.BotToken;
            this.Volume = cfg.BotVolume;
            this.OwnerId = cfg.BotOwnerId;
        }


        public string Token { get; set; }

        public float Volume { get; set; }


        public ulong OwnerId { get; set; }

    }
}
