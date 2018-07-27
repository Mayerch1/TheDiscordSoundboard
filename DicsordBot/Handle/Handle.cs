using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    public static class Handle
    {
        public static Data.RuntimeData Data { get; set; } = new Data.RuntimeData();
        public static Bot.BotHandle Bot { get; set; } = new Bot.BotHandle();

        public static BotData BotData { get; set; } = new BotData();

        #region events

        public static void ClientName_Changed(string newName)
        {
            var id = BotData.resolveUserName(newName);

            if (id.Result > 0)
                Data.Persistent.ClientId = id.Result;
        }

        #endregion events

        #region handle shared data

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