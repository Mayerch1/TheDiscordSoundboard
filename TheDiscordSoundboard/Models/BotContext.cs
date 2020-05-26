


namespace TheDiscordSoundboard.Models
{
    public class BotContext
    {
        private static Bot.BotState bot { get; set; } = new Bot.BotState();
        //private static Worker.Bot discordBot { get; set; } = new Worker.Bot();


        public Bot.BotState Bot { get => bot; set { bot = value; } }

        //public Worker.Bot DiscordBot { get => discordBot; set { discordBot = value; } }
    }
}
