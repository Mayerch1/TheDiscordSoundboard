using System;
using System.Collections.Generic;
using System.Text;

namespace TheDiscordSoundboard.Models.config
{
    public class ButtonConfigDto
    {
        public ButtonConfigDto() { }
        public ButtonConfigDto(Config cfg)
        {
            this.Width = cfg.ButtonWidth;
            this.Height = cfg.ButtonHeight;
        }

        public double Width { get; set; }
        public double Height { get; set; }
    }
}