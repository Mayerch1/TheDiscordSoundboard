using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DicsordBot.Data
{
    /// <summary>
    /// represents one file out of all local files,
    /// </summary>
    [Serializable()]
    public class FileData : INotifyPropertyChanged
    {
        private static uint globalId = 0;

        /// <summary>
        /// constructor, sets the unique id
        /// </summary>
        public FileData()
        {
            id = globalId++;
        }

        private string name = "";
        private string path = "";
        private string author = "";

        private readonly uint id;

        private TimeSpan length = TimeSpan.Zero;

        /// <summary>
        /// unique id of object
        /// </summary>
        public uint Id { get { return id; } }

        /// <summary>
        /// Name of file
        /// </summary>
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        /// <summary>
        /// path to file
        /// </summary>
        public string Path { get { return path; } set { path = value; OnPropertyChanged("Path"); } }

        /// <summary>
        /// Author of the title
        /// </summary>
        public string Author { get { return author; } set { author = value; OnPropertyChanged("Author"); } }

        /// <summary>
        /// the dateTime of the last replay
        /// </summary>
        [XmlIgnore]
        public TimeSpan Length { get { return length; } set { length = value; OnPropertyChanged("Length"); } }

        /// <summary>
        /// for serializing the Timespan
        /// </summary>
        public long LengthTicks
        {
            get { return length.Ticks; }
            set { length = new TimeSpan(value); }
        }

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