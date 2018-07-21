using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    public static class Handle
    {
        public static RuntimeData Data { get; set; } = new RuntimeData();
        public static BotHandle Bot { get; set; } = new BotHandle();
    }
}