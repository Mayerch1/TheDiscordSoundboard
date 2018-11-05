using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DataManagement
{
    /// <summary>
    /// represents one file out of all local files,
    /// </summary>
    [Serializable()]
    public class FileData : INotifyPropertyChanged
    {
        /// <summary>
        /// constructor, sets the unique id
        /// </summary>
        public FileData()
        {
            id = globalId++;
        }

        /// <summary>
        /// constructor, sets the unique id
        /// </summary>
        /// <param name="_path">path to file</param>
        public FileData(string _path)
        {
            path = _path;
            id = globalId++;
        }

        private static uint globalId = 0;

        private string name = "";
        private string path = "";
        private string author = "";
        private string album = "";
        private string genre = "";

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
        /// Album of the title
        /// </summary>
        public string Album { get { return album; } set { album = value; OnPropertyChanged("Album"); } }

        /// <summary>
        /// Genre of the title
        /// </summary>
        public string Genre { get { return genre; } set { genre = value; OnPropertyChanged("Genre"); } }

        /// <summary>
        /// the length of title as string mm:ss
        /// </summary>
        [XmlIgnore]
        public string LengthString { get { return (Length).ToString(@"mm\:ss"); } }

        /// <summary>
        /// the length of title as TimeSpan
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