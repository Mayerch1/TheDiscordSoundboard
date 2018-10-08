using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DicsordBot.Data
{
    /// <summary>
    /// stores all data which are permanentaly preserved
    /// </summary>
    [Serializable()]
    public class PersistentData : INotifyPropertyChanged
    {
        #region consts

        /// <summary>
        /// minVisibleButtons field
        /// </summary>
        /// <remarks>
        /// the count of buttons which are shown, even if there are less buttons used
        /// </remarks>
        public const int minVisibleButtons = 35;

        /// <summary>
        /// this is the url pointing at the git repository of this project. It is mainly referenced for help buttons
        /// </summary>
        public const string urlToGitRepo = "https://github.com/mayerch1/TheDiscordSoundboard/";

        /// <summary>
        /// default folder to create, e.g. in Appdata
        /// </summary>
        public const string defaultFolderName = "TheDicsordSoundboard (TDS)";

        /// <summary>
        /// folder to cache images in
        /// </summary>
        public const string imageCacheFolder = "Images";

        /// <summary>
        /// version of this build, refers to the github release number
        /// </summary>
        public const string version = "2.0.0";

        /// <summary>
        /// a list with all supported formats (only ending)
        /// </summary>
        [XmlIgnore]
        public readonly List<string> supportedFormats = new List<string> { "mp3", "wav", "asf", "wma", "wmv", "sami", "smi", "3g2", "3gp", "3pg2", "3pgg", "aac", "adts", "m4a", "m4v", "mov", "mp4" };

        #endregion consts

        #region persistend fields

        private ObservableCollection<ButtonData> btnList = new ObservableCollection<ButtonData>();
        private ObservableCollection<string> mediaSources = new ObservableCollection<string>();
        private ObservableCollection<FileData> playListIndex = new ObservableCollection<FileData>();
        private ObservableCollection<Hotkey> hotkeyList = new ObservableCollection<Hotkey>();

        private bool isFirstStart = true;

        private string settingsPath;
        private int highestButtonToSave = -1;
        private ulong clientId;
        private string clientName;
        private bool isDarkTheme = false;
        private ulong channelId = 0;
        private string clientAvatar;
        private string token = null;
        private int selectedServerIndex = 0;

        private bool ignoreFileWarning = false;
        private bool ignoreTokenWarning = false;
        private bool ignoreChannelWarning = false;
        private bool ignoreClientWarning = false;

        private float volume = 0.5f;
        private int volumeCap = 30;

        #endregion persistend fields

        #region persistend properties

        /// <summary>
        /// IsFirstStart property
        /// </summary>
        /// <value>
        /// if true, introduction guides will be loaded
        /// </value>
        public bool IsFirstStart { get { return isFirstStart; } set { isFirstStart = value; OnPropertyChanged("IsFirstStart"); } }

        /// <summary>
        ///  SettingsPath property, automatically saved
        /// </summary>
        public string SettingsPath { get { return settingsPath; } set { settingsPath = value; Properties.Settings.Default.Path = value; OnPropertyChanged("SettingsPath"); } }

        /// <summary>
        ///  HihgestButtonToSave property
        /// </summary>
        /// <value>
        /// all buttons above this number are empty
        /// </value>
        public int HighestButtonToSave { get { return highestButtonToSave; } set { highestButtonToSave = value; OnPropertyChanged("HighestButtonToSave"); } }

        /// <summary>
        ///  ClientId property
        /// </summary>
        public ulong ClientId { get { return clientId; } set { clientId = value; OnPropertyChanged("ClientId"); } }

        /// <summary>
        /// ChannelId property
        /// </summary>
        /// <value>
        /// target channel to join, set to 0 to join to owners channel
        /// </value>
        public ulong ChannelId { get { return channelId; } set { channelId = value; OnPropertyChanged("ChannelId"); } }

        /// <summary>
        ///  ClientAvatar property
        /// </summary>
        /// <value>
        /// url to image avatar image
        /// </value>
        public string ClientAvatar { get { return clientAvatar; } set { clientAvatar = value; OnPropertyChanged("ClientAvatar"); } }

        /// <summary>
        /// IsDarkTheme property
        /// </summary>
        public bool IsDarkTheme { get { return isDarkTheme; } set { isDarkTheme = value; OnPropertyChanged("IsDarkTheme"); } }

        //raise ClientNameChanged to check for client names, if old Token was not able to do so
        /// <summary>
        /// Token property
        /// </summary>
        public string Token { get { return token; } set { token = value; OnPropertyChanged("Token"); if (ClientNameChanged != null) ClientNameChanged(value); } }

        /// <summary>
        /// SelectedServerIndex
        /// </summary>
        /// <value>
        /// the index of the server, which channles are displayed in the channel selector
        /// </value>
        public int SelectedServerIndex { get { return selectedServerIndex; } set { selectedServerIndex = value; OnPropertyChanged("SelectedServerIndex"); } }

        /// <summary>
        /// Volume property
        /// </summary>
        /// <value>
        /// stores volume from 0.0 to 1.0
        /// </value>
        public float Volume { get { return volume; } set { volume = value; OnPropertyChanged("Volume"); } }

        /// <summary>
        /// VolumeCap property
        /// </summary>
        /// <value>
        /// limits the volume from 0 to 100 percent
        /// </value>
        public int VolumeCap { get { return volumeCap; } set { volumeCap = value; OnPropertyChanged("VolumeCap"); } }

        /// <summary>
        /// ClientName property
        /// </summary>
        /// <value>
        /// discord username in form of 'Name#1234'
        /// </value>
        public string ClientName { get { return clientName; } set { clientName = value; OnPropertyChanged("ClientName"); if (ClientNameChanged != null) ClientNameChanged(value); } }

        /// <summary>
        /// IgnoreFileWarning property
        /// </summary>
        public bool IgnoreFileWarning { get { return ignoreFileWarning; } set { ignoreFileWarning = value; OnPropertyChanged("IgnoreFileWarning"); } }

        /// <summary>
        /// IgnoreTokenWarning property
        /// </summary>
        public bool IgnoreTokenWarning { get { return ignoreTokenWarning; } set { ignoreTokenWarning = value; OnPropertyChanged("IgnoreTokenWarning"); } }

        /// <summary>
        /// IgnoreChannelWarning property
        /// </summary>
        public bool IgnoreChannelWarning { get { return ignoreChannelWarning; } set { ignoreChannelWarning = value; OnPropertyChanged("IgnoreChannelWarning"); } }

        /// <summary>
        /// IgnoreClientWarning property
        /// </summary>
        public bool IgnoreClientWarning { get { return ignoreClientWarning; } set { ignoreClientWarning = value; OnPropertyChanged("IgnoreClientWarning"); } }

        /// <summary>
        /// MediaSources property
        /// </summary>
        /// <value>
        /// list of all locations to monitor for files
        /// </value>
        public ObservableCollection<string> MediaSources { get { return mediaSources; } set { mediaSources = value; OnPropertyChanged("MediaSources"); } }

        /// <summary>
        ///name and directory of each playlist, used for loading the files
        /// </summary>
        public ObservableCollection<FileData> PlayListIndex { get { return playListIndex; } set { playListIndex = value; OnPropertyChanged("PlaylistFiles"); } }

        /// <summary>
        /// List of all registered hotkeys
        /// </summary>
        public ObservableCollection<Hotkey> HotkeyList { get { return hotkeyList; } set { hotkeyList = value; OnPropertyChanged("HotkeyList"); } }

        /// <summary>
        /// BtnList property
        /// </summary>
        /// <value>
        /// list of all button-data elements
        /// </value>
        public ObservableCollection<ButtonData> BtnList { get { return btnList; } set { btnList = value; OnPropertyChanged("BtnList"); } }

        #endregion persistend properties

        #region events

        /// <summary>
        /// ClientNameHandler delegate
        /// </summary>
        /// <param name="newName">new owner Name</param>
        public delegate void ClientNameHandler(string newName);

        /// <summary>
        /// ClientNameHandler event, if client name has changed
        /// </summary>
        public event ClientNameHandler ClientNameChanged;

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