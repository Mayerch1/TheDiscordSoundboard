using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DataManagement
{
    /// <summary>
    /// Represents on Button with all its properties, implements INotifyPropertyChanged
    /// </summary>
    [Serializable()]
    public class ButtonData : INotifyPropertyChanged

    {
        #region constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public ButtonData()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_file">path to file</param>
        public ButtonData(string _file)
        {
            File = _file;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_name">name of file</param>
        /// <param name="_file">path to file</param>
        public ButtonData(string _name, string _file)
        {
            Name = _name;
            File = _file;
        }

        #endregion constructors

        #region saved fields
        private string name = null;
        private string file = null;
        private bool isEarrape = false;
        private bool isLoop = false;
        private int iD;
        private uint hotkey_vk = 0;
        private uint hotkey_mod = 0;

        #endregion saved fields

        #region propertys

        //no OnPropertyChanged,
        //as change of value is only possible when not loaded (in settings)
        [XmlIgnore]
        public static double Width { get; set; }

        [XmlIgnore]
        public static double Height { get; set; }
           

        /// <summary>
        /// Name property
        /// </summary>
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        /// <summary>
        /// File property
        /// </summary>
        /// <value>
        /// path towards file in filesystem
        /// </value>
        public string File { get { return file; } set { file = value; OnPropertyChanged("File"); } }

        /// <summary>
        /// IsEarrape property
        /// </summary>
        /// <value>
        /// determins if boost should be applied
        /// </value>
        public bool IsEarrape { get { return isEarrape; } set { isEarrape = value; OnPropertyChanged("IsEarrape"); } }

        /// <summary>
        /// IsLoop property
        /// </summary>
        public bool IsLoop { get { return isLoop; } set { isLoop = value; OnPropertyChanged("IsLoop"); } }

        /// <summary>
        /// virtual keycode of assigned hotkey
        /// </summary>
        public uint Hotkey_VK { get { return hotkey_vk; } set { hotkey_vk = value; OnPropertyChanged("Hotkey_VK"); } }

        /// <summary>
        /// modifier code of assigned hotkey
        /// </summary>
        public uint Hotkey_MOD { get { return hotkey_mod; } set { hotkey_mod = value; OnPropertyChanged("Hotkey_MOD"); } }

        /// <summary>
        /// ID property
        /// </summary>
        /// <value>
        /// incremental button id, also stored in tag of btn to assign ui-button to data-button
        /// </value>
        public int ID { get { return iD; } set { iD = value; OnPropertyChanged("ID"); } }

        #endregion propertys

        #region event

        /// <summary>
        /// PropertyChanged Event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// propertychanged method, notifies the actual handler
        /// </summary>
        /// <param name="info"></param>
        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event
    }
}