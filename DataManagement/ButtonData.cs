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
        public new long Id { get => base.id; set { base.id = value; OnPropertyChanged("Id"); } }


        /// <summary>
        /// do not update database, as only change on index is of interest
        /// change of member of track is handled by TrackData class
        /// </summary>
        [JsonIgnore]
        public new TrackData Track { get => base.track; set { base.track = value; OnPropertyChanged("Track"); } }


        [JsonIgnore]
        public new string NickName {
            get=>base.nick_name;
            set {  
                if (value != base.nick_name)
                {
                    base.nick_name = value;
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
            get => base.is_earrape;
            set
            {
                if (value != base.is_earrape)
                {
                    base.is_earrape = value;
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
            get => base.is_loop;
            set
            {
                if (value != base.is_loop)
                {
                    base.is_loop = value;
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
            get => base.position;
            set
            {
                if (value != base.position)
                {
                    base.position = value;
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
            get => base.track_id;
            set
            {
                if (value != base.track_id)
                {
                    base.track_id = value;
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