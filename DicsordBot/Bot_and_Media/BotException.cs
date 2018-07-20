using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    internal class BotException : Exception
    {
        public enum type { connection, file, others }

        public enum connectionError { NoServer, NoChannel, NoStream, Unspecified }

        public type Type { get; set; }
        public connectionError ConnectionError { get; set; }

        public BotException(type _type, string _msg, connectionError _conType = connectionError.Unspecified) : base(_msg)
        {
            Type = _type;
            ConnectionError = _conType;
        }
    }
}