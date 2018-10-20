using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DiscordBot.Data
{
    /// <summary>
    /// represents a playlist
    /// </summary>
    [Serializable()]
    public class Playlist : INotifyPropertyChanged
    {
        #region consts

        /// <summary>
        /// default thumbnail, to use if nothing else is set
        /// </summary>
        public const string defaultImage = "/res/list-256.png";

        #endregion consts

        /// <summary>
        /// default constructor
        /// </summary>
        public Playlist()
        {
            id = sId++;
        }

        /// <summary>
        /// constructor setting the name property
        /// </summary>
        /// <param name="_name"></param>
        public Playlist(string _name)
        {
            id = sId++;
            name = _name;
        }

        /// <summary>
        /// constructor setting the name and image property
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_path"></param>
        public Playlist(string _name, string _path)
        {
            id = sId++;
            Name = _name;
            if (_path != null)
                ImagePath = _path;
        }

        #region fileds

        private static uint sId = 0;

        private ObservableCollection<FileData> tracks = new ObservableCollection<FileData>();
        private string name = "";
        private string imagePath = defaultImage;
        private readonly uint id;

        #endregion fileds

        #region properties

        /// <summary>
        /// Property for tracks of playlist
        /// </summary>
        public ObservableCollection<FileData> Tracks { get { return tracks; } set { tracks = value; OnPropertyChanged("Tracks"); } }

        /// <summary>
        /// Name property
        /// </summary>
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        /// <summary>
        /// ImagePath property
        /// </summary>
        public string ImagePath { get { return imagePath; } set { imagePath = value; OnPropertyChanged("ImagePath"); } }

        /// <summary>
        /// unique id
        /// </summary>
        public uint Id { get { return id; } }

        #endregion properties

        #region events

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
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(info));
            
        }

        #endregion events
    }
}