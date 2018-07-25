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
        private string file = null;
        private bool isEarrape = false;
        private bool isLoop = false;
        private int iD;

        #endregion saved fields

        #region propertys

        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public string File { get { return file; } set { file = value; OnPropertyChanged("File"); } }

        public bool IsEarrape { get { return isEarrape; } set { isEarrape = value; OnPropertyChanged("IsEarrape"); } }

        public bool IsLoop { get { return isLoop; } set { isLoop = value; OnPropertyChanged("IsLoop"); } }

        public int ID { get { return iD; } set { iD = value; OnPropertyChanged("ID"); } }

        #endregion propertys

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