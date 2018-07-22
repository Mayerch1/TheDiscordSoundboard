using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DicsordBot.Data
{
    [Serializable()]
    public class ButtonData
    {
        #region visible Button preference

        public string Name { get; set; } = null;

        public string BorderBrushString { get; set; } = "#FFDDDDDD";

        public string BackgroundBrushString { get; set; } = "#FF000000";

        public string ForegroundBrushString { get; set; } = "#FF707070";

        public string FontString { get; set; } = "Segoe UI";

        #endregion visible Button preference

        #region Button data preference

        public string File { get; set; } = null;

        public bool IsEarrape { get; set; } = false;

        public bool IsLoop { get; set; } = false;

        public int ID { get; set; }

        #endregion Button data preference

        #region noXml

        [XmlIgnore]
        public FontFamily Font
        {
            get { return new FontFamily(FontString); }
        }

        [XmlIgnore]
        public Brush BorderBrush
        {
            get { return (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(BorderBrushString); }
        }

        [XmlIgnore]
        public Brush BackgroundBrush
        {
            get { return (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(BackgroundBrushString); }
        }

        [XmlIgnore]
        public Brush ForegroundBrush
        {
            get { return (Brush)new System.Windows.Media.BrushConverter().ConvertFromString(ForegroundBrushString); }
        }

        #endregion noXml
    }
}