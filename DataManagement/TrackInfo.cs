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
            get => base.name;
            set
            {
                if (value != base.name)
                {
                    base.name = value;
                    OnPropertyChanged("Name");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string LocalFile
        {
            get => base.local_file;
            set
            {
                if (value != base.local_file)
                {
                    base.local_file = value;
                    OnPropertyChanged("LocalFile");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Uri
        {
            get => base.uri;
            set
            {
                if (value != base.uri)
                {
                    base.uri = value;
                    OnPropertyChanged("Uri");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string ImageUri
        {
            get => base.image_uri;
            set
            {
                if (value != base.image_uri)
                {
                    base.image_uri = value;
                    OnPropertyChanged("ImageUri");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Description
        {
            get => base.description;
            set
            {
                if (value != base.description)
                {
                    base.description = value;
                    OnPropertyChanged("Description");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Author
        {
            get => base.author;
            set
            {
                if (value != base.author)
                {
                    base.author = value;
                    OnPropertyChanged("Author");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Album
        {
            get => base.album;
            set
            {
                if (value != base.album)
                {
                    base.album = value;
                    OnPropertyChanged("Album");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new string Genre
        {
            get => base.genre;
            set
            {
                if (value != base.genre)
                {
                    base.genre = value;
                    OnPropertyChanged("Genre");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new int Duration
        {
            get => base.duration;
            set
            {
                if (value != base.duration)
                {
                    base.duration = value;
                    OnPropertyChanged("Duration");
                    if (TrackDataUpdated != null)
                    {
                        // id can change on POST
                        this.id = TrackDataUpdated(this);
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
