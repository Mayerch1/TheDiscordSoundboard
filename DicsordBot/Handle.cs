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

        #endregion events

        #region Snackbar

        /// <summary>
        /// SnackbarAction enum, passed into delegate for informing eventHandler on requested action
        /// </summary>
        public enum SnackbarAction
        {
            /// <summary>
            /// Open the Settings menu, when clicked
            /// </summary>
            Settings,

            /// <summary>
            /// Open the update page, when clicked
            /// </summary>
            Update,

            /// <summary>
            /// Do nothing on click
            /// </summary>
            None
        };

        /// <summary>
        /// SnackbarWarningHandle
        /// </summary>
        /// <param name="msg">Error decsription</param>
        /// <param name="action">Enum for triggered action in shown Snackbar</param>
        public delegate void SnackarWarningHandle(string msg, SnackbarAction action = SnackbarAction.None);

        /// <summary>
        /// SnackbarWarning instance
        /// </summary>
        public static SnackarWarningHandle SnackbarWarning;

        /// <summary>
        /// used to convert old Bot SnackbarWarnings to new universal warnings
        /// </summary>
        public static void PassBotSnackbarWarning(string msg, Bot.BotHandle.SnackbarAction action)
        {
            int converter = (int)action;

            SnackbarAction convertAction = (SnackbarAction)converter;

            SnackbarWarning(msg, convertAction);
        }

        #endregion Snackbar

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