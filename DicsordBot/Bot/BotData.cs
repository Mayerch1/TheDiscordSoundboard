using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot.Bot
{
#pragma warning disable CS1591

    /// <summary>
    /// Contains data for streaming
    /// </summary>
    public class BotData
    {
        public BotData()
        {
        }

        public BotData(string _name, string _filePath)
        {
            name = _name;
            filePath = _filePath;
        }

        public string name = "";
        public string filePath = null;
        public byte[] stream = null;
        public bool isEarrape = false;
        public bool isLoop = false;
        public int id = -1;
    }

#pragma warning restore CS1591
}