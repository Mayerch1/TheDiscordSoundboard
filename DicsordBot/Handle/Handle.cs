using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DicsordBot
{
    /// <summary>
    /// Static Handle contains Bot and Data instance, also provides synced properties and methods to show warnings/errors
    /// </summary>
    public static class Handle
    {
        /// <summary>
        /// Data class, handles and contains runtime + persistent Data
        /// </summary>
        public static Data.RuntimeData Data { get; set; } = new Data.RuntimeData();

        /// <summary>
        /// Bot class, handles all commands towards bot
        /// </summary>
        public static Bot.BotHandle Bot { get; set; } = new Bot.BotHandle();

        /// <summary>
        /// Provides methods depending on Bot, but are only used to get data, not to perform actions on bot
        /// </summary>
        public static BotData BotData { get; set; } = new BotData();

        #region events

        /// <summary>
        /// event method, for changed clientname, triggers update of user id
        /// </summary>
        /// <param name="newName">
        /// new ClientName 'Name#1234'
        /// </param>
        public static async void ClientName_Changed(string newName)
        {
            var id = await BotData.resolveUserName(newName);

            if (id > 0)
                Data.Persistent.ClientId = id;
        }

        /// <summary>
        /// shows small warning window for middle-severe errors,
        /// </summary>
        /// <param name="title">windows title</param>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution to solve the error</param>
        /// <returns>bool for future ignoring this error</returns>
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

        /// <summary>
        /// show small warning windows for middle-severe file errors
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution to solve the error</param>
        public static void FileWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreFileWarning)
            {
                Data.Persistent.IgnoreFileWarning = ShowWarning("A File-Error occurred", msg, solution);
            }
        }

        /// <summary>
        /// show small warning windows for middle-severe channel errors
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution to solve the error</param>
        public static void ChannelWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreChannelWarning)
            {
                Data.Persistent.IgnoreChannelWarning = ShowWarning("A Channel-Error occurred", msg, solution);
            }
        }

        /// <summary>
        /// show small warning windows for middle-severe token errors
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution to solve the error</param>
        public static void TokenWarning_Show(string msg, string solution)
        {
            if (!Data.Persistent.IgnoreTokenWarning)
            {
                Data.Persistent.IgnoreTokenWarning = ShowWarning("A Token-Error occurred", msg, solution);
            }
        }

        /// <summary>
        /// show small warning windows for middle-severe client errors
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution to solve the error</param>
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
        /// property for Volume, synced
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

        /// <summary>
        /// property for Token, synced
        /// </summary>
        public static string Token
        {
            get { return Data.Persistent.Token; }
            set
            {
                Bot.Token = value;
                Data.Persistent.Token = value;
            }
        }

        /// <summary>
        /// property for ChannelId, synced
        /// </summary>
        public static ulong ChannelId
        {
            get { return Data.Persistent.ChannelId; }
            set
            {
                Bot.ChannelId = value;
                Data.Persistent.ChannelId = value;
            }
        }

        /// <summary>
        /// property for ClientId, synced
        /// </summary>
        public static ulong ClientId
        {
            get { return Data.Persistent.ClientId; }
            set
            {
                Bot.ClientId = value;
                Data.Persistent.ClientId = value;
            }
        }

        /// <summary>
        /// property for ClientName, synced
        /// </summary>
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