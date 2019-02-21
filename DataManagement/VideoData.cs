using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeSearch;

namespace DataManagement
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
        /// <param name="duration">duration of the video</param>
        /// <param name="description">video description</param>
        /// <param name="author">Author or Uploader of Video</param>
        public VideoData(string url, string title, string author, string imageUrl, string duration = "", string description="")
        {
            Title = title;
            Url = url;
            ImageUrl = imageUrl;
            Duration = duration;
            Description = description;
            Author = author;
        }

        /// <summary>
        /// sets all properties given from input
        /// </summary>
        /// <param name="info">VideoInformation object, usually gathered from YT Query-search</param>
        public VideoData(VideoInformation info)
        {
            Title = info.Title;
            Url = info.Url;
            ImageUrl = info.Thumbnail;
            Duration = info.Duration;
            Description = info.Description;
            Author = info.Author;

        }

        private string _title = "";
        private string _url = "";
        private string _imageUrl = "";
        private string _description = "";
        private string _duration = "0:00";
        private string _author = "";

         /// <summary>
         /// Title of video
         /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// url to video
        /// </summary>
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged("Url");
            }
        }
        /// <summary>
        /// url to thumbnail of video
        /// </summary>
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged("ImageUrl");
            }
        }

        /// <summary>
        /// Duration string property
        /// </summary>
        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }


        /// <summary>
        /// Author or uploader of video
        /// </summary>
        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged("Author");
            }
        }


        /// <summary>
        /// Video Description Property
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged("Description");
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
