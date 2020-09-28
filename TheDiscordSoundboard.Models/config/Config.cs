using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TheDiscordSoundboard.Models.config
{
    public class Config
    {
        public void Update(BotConfigDto cfg)
        {
            this.BotToken = cfg.Token;
            this.BotVolume = cfg.Volume;
            this.BotOwnerId = cfg.OwnerId;
        }

        public void Update(ButtonConfigDto cfg)
        {
            this.ButtonHeight = cfg.Height;
            this.ButtonWidth = cfg.Width;
        }


        public long Id { get; set; }

        //=================================
        //          Bot Config
        //=================================
        public string BotToken { get; set; }
        public float BotVolume { get; set; } = 1.0f;
        public ulong BotOwnerId { get; set; } = 0;


        //=================================
        //          Button Config
        //=================================
        public double ButtonWidth { get; set; }
        public double ButtonHeight { get; set; }
    }
}
