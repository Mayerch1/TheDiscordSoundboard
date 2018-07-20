using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot
{
    [Serializable()]
    public class ButtonData
    {
        public string Name { get; set; }
        public string File { get; set; }

        public float Volume { get; set; }

        public bool IsLoop { get; set; }

        public int ID { get; set; }

        public string BorderBrush { get; set; }

        public string BackgroundBrush { get; set; }

        #region static vars

        public static string ForegroundBrush { get; set; }

        public static string FontFamily { get; set; }

        #endregion static vars

        //settings per button go here
    }
}