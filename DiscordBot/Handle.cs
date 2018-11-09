using System;
using System.Windows.Threading;
using BotModule;
using DiscordBot.Misc;
using Newtonsoft.Json.Serialization;

namespace DiscordBot
{
    /// <summary>
    /// Static Handle contains Bot and Data instance, also provides synced properties and methods to show warnings/errors
    /// </summary>
    public static class Handle
    {
        /// <summary>
        /// Data class, handles and contains runtime + persistent Data
        /// </summary>
        public static DataManagement.RuntimeData Data { get; set; } = new DataManagement.RuntimeData();

        /// <summary>
        /// Bot class, handles all commands towards bot
        /// </summary>
        public static BotHandle Bot { get; set; } = new BotHandle();

        /// <summary>
        /// Provides methods depending on Bot, but are only used to get data, not to perform actions on bot
        /// </summary>
        public static BotMisc BotMisc { get; set; } = new BotMisc();

   

        #region events

        /// <summary>
        /// event method, for changed clientname, triggers update of user id
        /// </summary>
        /// <param name="newName">
        /// new ClientName 'Name#1234'
        /// </param>
        public static async void ClientName_Changed(string newName)
        {
            var id = await BotMisc.resolveUserName(newName);

            if (id > 0)
                Data.Persistent.ClientId = id;
        }

        #endregion events

        #region Snackbar converter
        /// <summary>
        /// used to convert old Bot SnackbarWarnings to new universal warnings
        /// </summary>
        public static void PassBotSnackbarWarning(string msg, BotHandle.SnackbarAction action)
        {
            int converter = (int)action;

            Util.IO.SnackbarManager.SnackbarAction convertAction = (Util.IO.SnackbarManager.SnackbarAction)converter;

            Util.IO.SnackbarManager.SnackbarMessage(msg, convertAction);
        }
      

        #endregion Snackbar converter

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
                var uId = BotMisc.resolveUserName(value);
                if (uId.Result > 0)
                    ClientId = uId.Result;
            }
        }

        #endregion handle shared data
    }
}