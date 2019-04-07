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
        /// event method, for changed client name, triggers update of user id
        /// </summary>
        /// <param name="newName">
        /// new ClientName 'Name#1234'
        /// </param>
        public static async void ClientName_Changed(string newName)
        {
            if (!String.IsNullOrWhiteSpace(newName))
            {
                var id = await BotMisc.resolveUserName(newName);

                if (id > 0)
                    ClientId = id;

            }
        }

        #endregion events   

        #region handle shared data

        /// <summary>
        /// property for Volume, synced
        /// </summary>
        public static float Volume
        {
            get => Data.Persistent.Volume;
            set
            {
                Bot.Volume = value;
                Data.Persistent.Volume = value;
            }
        }

        /// <summary>
        /// sync property for pitch
        /// </summary>
        public static float Pitch
        {
            //pitch is volatile
            get => Data.Pitch;
            //pitch will not be saved
            set
            {
                Bot.Pitch = value;
                Data.Pitch = value;
            }
        }

        /// <summary>
        /// property for Token, synced
        /// </summary>
        public static string Token
        {
            get => Data.Persistent.Token;
            set
            {
                Bot.Token = value;
                Data.Persistent.Token = value;
                //trigger this on every change of Credential/Username 
                ClientName_Changed(ClientName);
            }
        }

        /// <summary>
        /// property for ChannelId, synced
        /// </summary>
        public static ulong ChannelId
        {
            get => Data.Persistent.ChannelId;
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
            get => Data.Persistent.ClientId;
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
            get => Data.Persistent.ClientName;
            set
            {
                //set clientId, by resolving userName
                Data.Persistent.ClientName = value;
                //trigger this on every change of Credential/Username 
                ClientName_Changed(value);
            }
        }

        #endregion handle shared data
    }
}