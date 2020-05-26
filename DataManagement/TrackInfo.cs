using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models;

namespace DataManagement
{
    public class TrackInfo: TrackData, INotifyPropertyChanged
    {
        public delegate long OnTrackDataUpdate(TrackData update);
        [JsonIgnore]
        public static OnTrackDataUpdate TrackDataUpdated;


        [JsonIgnore]
        public new string Name
        {
            get => base.Name;
            set
            {
                if (value != base.Name)
                {
                    base.Name = value;
                    OnPropertyChanged("Name");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string LocalFile
        {
            get => base.LocalFile;
            set
            {
                if (value != base.LocalFile)
                {
                    base.LocalFile = value;
                    OnPropertyChanged("LocalFile");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Uri
        {
            get => base.Uri;
            set
            {
                if (value != base.Uri)
                {
                    base.Uri = value;
                    OnPropertyChanged("Uri");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string ImageUri
        {
            get => base.ImageUri;
            set
            {
                if (value != base.ImageUri)
                {
                    base.ImageUri = value;
                    OnPropertyChanged("ImageUri");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Description
        {
            get => base.Description;
            set
            {
                if (value != base.Description)
                {
                    base.Description = value;
                    OnPropertyChanged("Description");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Author
        {
            get => base.Author;
            set
            {
                if (value != base.Author)
                {
                    base.Author = value;
                    OnPropertyChanged("Author");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Album
        {
            get => base.Album;
            set
            {
                if (value != base.Album)
                {
                    base.Album = value;
                    OnPropertyChanged("Album");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Genre
        {
            get => base.Genre;
            set
            {
                if (value != base.Genre)
                {
                    base.Genre = value;
                    OnPropertyChanged("Genre");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new int Duration
        {
            get => base.Duration;
            set
            {
                if (value != base.Duration)
                {
                    base.Duration = value;
                    OnPropertyChanged("Duration");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.Id = TrackDataUpdated(this);
                    }
                }
            }
        }




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
    }
}
