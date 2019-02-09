using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DataManagement
{
    /// <summary>
    /// Class for VideoHistory
    /// </summary>
    public class VideoHistory: INotifyPropertyChanged
    {

        /// <summary>
        /// default constructor
        /// </summary>
        public VideoHistory()
        { }


        /// <summary>
        /// adds video to history, respects maxHistoryLen
        /// </summary>
        /// <param name="vid"></param>
        /// /// <param name="maxLen">max. number of files in list</param>
        public void addVideo(VideoData vid, int maxLen)
        {
            //see if video was already played
            var oldVid = videos.FirstOrDefault(x => x.Url == vid.Url);
          

            //remove video
            if (oldVid != null)
            {
                videos.Remove(oldVid);
            }

            //insert at top of list
            videos.Insert(0, vid);
            

            //delete videos above limit
            while(videos.Count > maxLen)
                videos.RemoveAt(videos.Count-1);
        }


        private ObservableCollection<VideoData> videos = new ObservableCollection<VideoData>();


        /// <summary>
        /// the recently played videos
        /// </summary>
        public ObservableCollection<VideoData> Videos
        {
            get => videos;
            set
            {
                videos = value;
                OnPropertyChanged("Videos");
            }
        }


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
