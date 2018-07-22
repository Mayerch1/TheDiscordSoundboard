using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DicsordBot.Data
{
    public class SerialFont
    {
        [XmlIgnore]
        public FontFamily FontValue { get; set; }
    }
}