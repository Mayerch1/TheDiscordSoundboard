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
        #region saved fields

        private string name = null;
        private string borderBrushString = "#FFDDDDDD";
        private string backgroundBrushString = "#FF000000";
        private string foregroundBrushString = "#FF707070";
        private string fontString = "Segoe UI";
        private string file = null;
        private bool isEarrape = false;
        private bool isLoop = false;
        private int iD;

        #endregion saved fields

        #region propertys

        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public string BorderBrushString { get { return borderBrushString; } set { borderBrushString = value; OnPropertyChanged("BorderBrushString"); } }
        public string BackgroundBrushString { get { return backgroundBrushString; } set { backgroundBrushString = value; OnPropertyChanged("BackgroundBrushString"); } }
        public string ForegroundBrushString { get { return foregroundBrushString; } set { foregroundBrushString = value; OnPropertyChanged("ForegroundBrushString"); } }
        public string FontString { get { return fontString; } set { fontString = value; OnPropertyChanged("FontString"); } }

        public string File { get { return file; } set { file = value; OnPropertyChanged("File"); } }

        public bool IsEarrape { get { return isEarrape; } set { isEarrape = value; OnPropertyChanged("IsEarrape"); } }

        public bool IsLoop { get { return isLoop; } set { isLoop = value; OnPropertyChanged("IsLoop"); } }

        public int ID { get { return iD; } set { iD = value; OnPropertyChanged("ID"); } }

        #endregion propertys

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

        #region event

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event
    }
}