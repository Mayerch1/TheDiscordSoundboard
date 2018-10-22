using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Data
{
    /// <inheritdoc />
    /// <summary>
    /// contains all important links to video and thumbnail
    /// </summary>
    [Serializable()]
    public class VideoData: INotifyPropertyChanged
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public VideoData()
        { }

        /// <summary>
        /// sets all properties of class
        /// </summary>
        /// <param name="url">url to video</param>
        /// <param name="title">title of video </param>
        /// <param name="imageUrl">url to thumbnail</param>
        public VideoData(string url, string title, string imageUrl)
        {
            Title = title;
            Url = url;
            ImageUrl = imageUrl;
        }

        private string title = "";
        private string url = "";
        private string imageUrl = "";

         /// <summary>
         /// Title of video
         /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// url to video
        /// </summary>
        public string Url
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged("Url");
            }
        }
        /// <summary>
        /// url to thumbnail of video
        /// </summary>
        public string ImageUrl
        {
            get => imageUrl;
            set
            {
                imageUrl = value;
                OnPropertyChanged("ImageUrl");
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
