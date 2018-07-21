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
    }
}