using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DicsordBot
{
    public static class Handle
    {
        public static Data.RuntimeData Data { get; set; } = new Data.RuntimeData();
        public static Bot.BotHandle Bot { get; set; } = new Bot.BotHandle();

        public static BotData BotData { get; set; } = new BotData();

        #region events

        public static async void ClientName_Changed(string newName)
        {
            var id = await BotData.resolveUserName(newName);

            if (id > 0)
                Data.Persistent.ClientId = id;
        }

        public static bool ShowWarning(string title, string msg, string solution)
        {
            Window window = new Window
            {
                Title = title,
                Content = new Hint(msg, solution),
                Width = 400,
                Height = 250,
            };
            window.ShowDialog();
            return ((Hint)window.Content).IgnoreWarning;
        }

        public static void FileWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreFileWarning)
            {
                Data.Persistent.IgnoreFileWarning = ShowWarning("A File-Error occurred", msg, solution);
            }
        }

        public static void ChannelWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreChannelWarning)
            {
                Data.Persistent.IgnoreChannelWarning = ShowWarning("A Channel-Error occurred", msg, solution);
            }
        }

        public static void TokenWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreTokenWarning)
            {
                Data.Persistent.IgnoreTokenWarning = ShowWarning("A Token-Error occurred", msg, solution);
            }
        }

        public static void ClientWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreClientWarning)
            {
                Data.Persistent.IgnoreClientWarning = ShowWarning("A Client-Error occurred", msg, solution);
            }
        }

        #endregion events

        #region handle shared data

        /// <summary>
        /// propertys of this class will be synced between Data.Persistent and Bot
        /// </summary>
        public static float Volume
        {
            get { return Data.Persistent.Volume; }
            set
            {
                Bot.Volume = value;
                Data.Persistent.Volume = value;
            }
        }

        public static string Token
        {
            get { return Data.Persistent.Token; }
            set
            {
                Bot.Token = value;
                Data.Persistent.Token = value;
            }
        }

        public static ulong ChannelId
        {
            get { return Data.Persistent.ChannelId; }
            set
            {
                Bot.ChannelId = value;
                Data.Persistent.ChannelId = value;
            }
        }

        public static ulong ClientId
        {
            get { return Data.Persistent.ClientId; }
            set
            {
                Bot.ClientId = value;
                Data.Persistent.ClientId = value;
            }
        }

        public static string ClientName
        {
            get { return Data.Persistent.ClientName; }
            set
            {
                //set clientId, by resolving userName
                Data.Persistent.ClientName = value;
                var uId = BotData.resolveUserName(value);
                if (uId.Result > 0)
                    ClientId = uId.Result;
            }
        }

        #endregion handle shared data
    }
}