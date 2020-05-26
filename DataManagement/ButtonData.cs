using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

using RestSharp;
using TheDiscordSoundboard.Models;
using System.Web.UI.WebControls;

namespace DataManagement
{
    /// <summary>
    /// Represents on Button with all its properties, implements INotifyPropertyChanged
    /// </summary>
    [Serializable()]
    public class ButtonData : Buttons, INotifyPropertyChanged

    {
        // only pass base-class, as others are not represented in db
        // only register once at same time (set null before adding handler)
        public delegate long OnButtonUpdate(Buttons update);
        [JsonIgnore]
        public static OnButtonUpdate ButtonUpdated;
        


        /// <summary>
        /// uses OnPropertyChanged as id is bound to 'Tag'-field of UI-Button
        /// </summary>
        [JsonIgnore]
        public new long Id { get => base.Id; set { base.Id = value; OnPropertyChanged("Id"); } }


        /// <summary>
        /// do not update database, as only change on index is of interest
        /// change of member of track is handled by TrackData class
        /// </summary>
        [JsonIgnore]
        public new TrackData Track { get => base.Track; set { base.Track = value; OnPropertyChanged("Track"); } }


        [JsonIgnore]
        public new string NickName {
            get=>base.NickName;
            set {  
                if (value != base.NickName)
                {
                    base.NickName = value;
                    OnPropertyChanged("NickName");
                    if (ButtonUpdated != null)
                    {
                        // id can change on POST
                        this.Id = ButtonUpdated(this);
                    }
                }
            } 
        }

        [JsonIgnore]
        public new bool IsEarrape
        {
            get => base.IsEarrape;
            set
            {
                if (value != base.IsEarrape)
                {
                    base.IsEarrape = value;
                    OnPropertyChanged("IsEarrape");
                    if (ButtonUpdated != null)
                    {
                        // id can change on POST
                        this.Id = ButtonUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new bool IsLoop
        {
            get => base.IsLoop;
            set
            {
                if (value != base.IsLoop)
                {
                    base.IsLoop = value;
                    OnPropertyChanged("IsLoop");
                    if (ButtonUpdated != null)
                    {
                        // id can change on POST
                        this.Id = ButtonUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new long Position
        {
            get => base.Position;
            set
            {
                if (value != base.Position)
                {
                    base.Position = value;
                    OnPropertyChanged("Position");
                    if (ButtonUpdated != null)
                    {
                        // id can change on POST
                        this.Id = ButtonUpdated(this);
                    }
                }
            }
        }

        [JsonIgnore]
        public new long? TrackId
        {
            get => base.TrackId;
            set
            {
                if (value != base.TrackId)
                {
                    base.TrackId = value;
                    OnPropertyChanged("TrackId");
                    if (ButtonUpdated != null)
                    {
                        // id can change on POST
                        this.Id = ButtonUpdated(this);
                    }
                }
            }
        }



        //no OnPropertyChanged,
        //as change of value is only possible when not loaded (in settings)
        /// <summary>
        /// Width of a single Button
        /// </summary>
        [JsonIgnore]
        public static double Width { get; set; }

        /// <summary>
        /// Height of a single Button
        /// </summary>
        [JsonIgnore]
        public static double Height { get; set; }

        /// <summary>
        /// virtual keycode of assigned hotkey
        /// </summary>
        public uint Hotkey_VK { get { return 0; } set {} }

        /// <summary>
        /// modifier code of assigned hotkey
        /// </summary>
        public uint Hotkey_MOD { get { return 0; } set { } }

       
        #region event




        /// <summary>
        /// Invoke OnProperyChanged from the outside
        /// usefull when name was changed w/o triggering PropertyChanged
        /// </summary>
        public void NotifyNameChanged()
        {
            OnPropertyChanged("NickName");
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
        
        #endregion event
    }
}