


namespace TheDiscordSoundboard.Models
{
    public class BotContext
    {
        private static TheDiscordSoundboard.Bot.Bot bot { get; set; } = new TheDiscordSoundboard.Bot.Bot();

        public TheDiscordSoundboard.Bot.Bot Bot { get => bot; set { bot = value; } }
    }
}
