using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot.Data
{
    /// <summary>
    /// represents a playlist
    /// </summary>
    [Serializable()]
    public class Playlist : INotifyPropertyChanged
    {
        private ObservableCollection<FileData> tracks = new ObservableCollection<FileData>();

        /// <summary>
        /// Property for tracks of playlist
        /// </summary>
        public ObservableCollection<FileData> Tracks { get { return tracks; } set { tracks = value; OnPropertyChanged("Tracks"); } }

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
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        #endregion events
    }
}