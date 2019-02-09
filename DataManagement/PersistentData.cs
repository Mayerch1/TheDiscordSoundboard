using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace DataManagement
{
    /// <summary>
    /// stores all data which are permanentaly preserved
    /// </summary>
    [Serializable()]
    public class PersistentData : INotifyPropertyChanged
    {
        #region consts

        /// <summary>
        /// complete url to discord page where the bot is created
        /// </summary>
        public const string urlToBotRegister = "https://discordapp.com/login?redirect_to=%2Fdevelopers%2Fapplications%2Fme";


        /// <summary>
        /// complete url to git Repository
        /// </summary>
        public const string gitCompleteUrl = "https://github.com/Mayerch1/TheDiscordSoundboard/";


        /// <summary>
        /// this is the Name of the Github repository
        /// </summary>
        public const string gitRepo = "TheDiscordSoundboard";
        /// <summary>
        /// this is the account name of the repository owner
        /// </summary>
        public const string gitAuthor = "mayerch1";

  

        /// <summary>
        /// default folder to create, e.g. in Appdata
        /// </summary>
        public const string defaultFolderName = "TheDicsordSoundboard (TDS)";

        /// <summary>
        /// folder to cache images in
        /// </summary>
        public const string imageCacheFolder = "Images";

        /// <summary>
        /// folder to cache videos in
        /// </summary>
        public const string videoCacheFolder = "Videos";

        /// <summary>
        /// version of this build, refers to the github release number
        /// </summary>
        public const string version = "2.3.0";


        #endregion consts

        #region persistend fields

        private bool dontSave = false;

        private ObservableCollection<ButtonData> btnList = new ObservableCollection<ButtonData>();
        private ObservableCollection<string> mediaSources = new ObservableCollection<string>();
        private ObservableCollection<FileData> playListIndex = new ObservableCollection<FileData>();
        private ObservableCollection<Hotkey> hotkeyList = new ObservableCollection<Hotkey>();
        private ObservableCollection<string> supportedFormats = new ObservableCollection<string>();


        private bool isBotModule = true;
        private bool isPlaylistModule = true;
        private bool isStreamModule = true;

        private bool isFirstStart = true;
        private bool isEulaAccepted = false;

        private string settingsPath;
        private int highestButtonToSave = -1;
        private ulong clientId;
        private string clientName;
        private ulong channelId = 0;
        private string clientAvatar;
        private string token = null;
        private int selectedServerIndex = 0;
        private bool alwaysCacheVideo = true;


        private bool isDarkTheme = false;
        private string primarySwatch = null;
        private string secondarySwatch = null;


        private int minVisibleButtons = 35;
        private int maxHistoryLen = 50;
        private int maxVideoHistoryLen = 25;

        private float volume = 0.5f;
        private int volumeCap = 30;

        #endregion persistend fields

        #region persistend properties


        /// <summary>
        /// For debugging, will not save any changes
        /// </summary>
        public bool DontSave
        {
            get => dontSave;
            set
            {
                dontSave = value;
                OnPropertyChanged("DontSave");
            }
        }

        /// <summary>
        /// determines, if BotModule will be loaded
        /// </summary>
        public bool IsBotModule
        {
            get => isBotModule;
            set
            {
                isBotModule = value;
                OnPropertyChanged("IsBotModule");
            }
        }

        /// <summary>
        /// determines, if PlaylistModule will be loaded
        /// </summary>
        public bool IsPlaylistModule
        {
            get => isPlaylistModule;
            set
            {
                isPlaylistModule = value;
                OnPropertyChanged("IsPlaylistModule");
            }
        }

        /// <summary>
        /// determines, if StreamModule will be loaded
        /// </summary>
        public bool IsStreamModule
        {
            get => isStreamModule;
            set
            {
                isStreamModule = value;
                OnPropertyChanged("IsStreamModule");
            }
        }

 

        /// <summary>
        /// IsFirstStart property
        /// </summary>
        /// <value>
        /// if true, introduction guides will be loaded
        /// </value>
        public bool IsFirstStart { get => isFirstStart;
            set { isFirstStart = value; OnPropertyChanged("IsFirstStart"); } }


        /// <summary>
        /// IsEulaAccepted
        /// </summary>
        /// <value> if true, the user acknowledged the legal consequences of using stream function</value>
        public bool IsEulaAccepted
        {
            get => isEulaAccepted;
            set
            {
                isEulaAccepted = value;
                OnPropertyChanged("IsEulaAccepted");
            }
        }

        /// <summary>
        ///  SettingsPath property, automatically saved
        /// </summary>
        public string SettingsPath { get => settingsPath;
            set { settingsPath = value;  OnPropertyChanged("SettingsPath"); } }

        /// <summary>
        ///  HihgestButtonToSave property
        /// </summary>
        /// <value>
        /// all buttons above this number are empty
        /// </value>
        public int HighestButtonToSave { get => highestButtonToSave;
            set { highestButtonToSave = value; OnPropertyChanged("HighestButtonToSave"); } }

        /// <summary>
        ///  ClientId property
        /// </summary>
        public ulong ClientId { get => clientId;
            set { clientId = value; OnPropertyChanged("ClientId"); } }

        /// <summary>
        /// ChannelId property
        /// </summary>
        /// <value>
        /// target channel to join, set to 0 to join to owners channel
        /// </value>
        public ulong ChannelId { get => channelId;
            set { channelId = value; OnPropertyChanged("ChannelId"); } }

        /// <summary>
        ///  ClientAvatar property
        /// </summary>
        /// <value>
        /// url to image avatar image
        /// </value>
        public string ClientAvatar { get => clientAvatar;
            set { clientAvatar = value; OnPropertyChanged("ClientAvatar"); } }

        /// <summary>
        /// IsDarkTheme property
        /// </summary>
        public bool IsDarkTheme { get => isDarkTheme;
            set { isDarkTheme = value; new PaletteHelper().SetLightDark(value);  OnPropertyChanged("IsDarkTheme"); } }


        /// <summary>
        /// this is the main color scheme
        /// </summary>
        public string PrimarySwatch
        {
            get => primarySwatch;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    //find swatch in table
                    var newSwatch = new MaterialDesignColors.SwatchesProvider().Swatches.FirstOrDefault(sw => sw.Name == value);

                    if (newSwatch != null)
                    {
                        //replace old primary swatch
                        new PaletteHelper().ReplacePrimaryColor(newSwatch);
                        primarySwatch = value;
                        OnPropertyChanged("PrimarySwatch");
                    }                 
                }
            }
        }

        /// <summary>
        /// this is the secondary color scheme
        /// </summary>
        public string SecondarySwatch
        {
            get => secondarySwatch;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    //find swatch in table of defaults
                    var newSwatch = new MaterialDesignColors.SwatchesProvider().Swatches.FirstOrDefault(sw => sw.Name == value);

                    if (newSwatch != null)
                    {
                        //replace old primary swatch
                        new PaletteHelper().ReplaceAccentColor(newSwatch);
                        secondarySwatch = value;
                        OnPropertyChanged("SecondarySwatch");
                    }                                    
                }
            }
        }
   

        //raise ClientNameChanged to check for client names, if old Token was not able to do so
        /// <summary>
        /// Token property
        /// </summary>
        public string Token { get => token;
            set { token = value; OnPropertyChanged("Token");
                ClientNameChanged?.Invoke(value);
            } }

        /// <summary>
        /// SelectedServerIndex
        /// </summary>
        /// <value>
        /// the index of the server, which channles are displayed in the channel selector
        /// </value>
        public int SelectedServerIndex { get => selectedServerIndex;
            set { selectedServerIndex = value; OnPropertyChanged("SelectedServerIndex"); } }

        /// <summary>
        /// Volume property
        /// </summary>
        /// <value>
        /// stores volume from 0.0 to 1.0
        /// </value>
        public float Volume { get => volume;
            set { volume = value; OnPropertyChanged("Volume"); } }


        /// <summary>
        /// VolumeCap property
        /// </summary>
        /// <value>
        /// limits the volume from 0 to 100 percent
        /// </value>
        public int VolumeCap { get => volumeCap;
            set { volumeCap = value; OnPropertyChanged("VolumeCap"); } }

        /// <summary>
        /// ClientName property
        /// </summary>
        /// <value>
        /// discord username in form of 'Name#1234'
        /// </value>
        public string ClientName { get => clientName;
            set { clientName = value; OnPropertyChanged("ClientName");
                ClientNameChanged?.Invoke(value);
            } }

      
        /// <summary>
        /// Do not use Videostream, instead cache each video
        /// </summary>
        public bool AlwaysCacheVideo
        {
            get => alwaysCacheVideo;
            set { alwaysCacheVideo = value; OnPropertyChanged("AlwaysCacheVideo"); } }

   
        /// <summary>
        /// minVisibleButtons Property
        /// </summary>
        /// <remarks>
        /// the count of buttons which are shown, even if there are less buttons used
        /// </remarks>
        public int MinVisibleButtons
        {
            get => minVisibleButtons;
            set
            {
                minVisibleButtons = value;
                OnPropertyChanged("MinVisibleButtons");
            }
        }

        /// <summary>
        /// Max entries in file history
        /// </summary>
        public int MaxHistoryLen
        {
            get => maxHistoryLen;
            set
            {
                maxHistoryLen = value;
                OnPropertyChanged("MaxHistoryLen");
            }
        }

        /// <summary>
        /// Max entries in video history
        /// </summary>
        public int MaxVideoHistoryLen
        {
            get => maxVideoHistoryLen;
            set
            {
                maxVideoHistoryLen = value;
                OnPropertyChanged("MaxVideoHistoryLen");
            }
        }


        /// <summary>
        /// MediaSources property
        /// </summary>
        /// <value>
        /// list of all locations to monitor for files
        /// </value>
        public ObservableCollection<string> MediaSources { get => mediaSources;
            set { mediaSources = value; OnPropertyChanged("MediaSources"); } }



        /// <summary>
        /// a list with all supported formats (only file ending, without .)
        /// </summary>
        public ObservableCollection<string> SupportedFormats
        {
            get => supportedFormats;
            set
            {
                supportedFormats = value;
                OnPropertyChanged("SupportedFormats");
            }
        }


        /// <summary>
        ///name and directory of each playlist, used for loading the files
        /// </summary>
        public ObservableCollection<FileData> PlayListIndex { get => playListIndex;
            set { playListIndex = value; OnPropertyChanged("PlaylistFiles"); } }

        /// <summary>
        /// List of all registered hotkeys
        /// </summary>
        public ObservableCollection<Hotkey> HotkeyList { get => hotkeyList;
            set { hotkeyList = value; OnPropertyChanged("HotkeyList"); } }

        /// <summary>
        /// BtnList property
        /// </summary>
        /// <value>
        /// list of all button-data elements
        /// </value>
        public ObservableCollection<ButtonData> BtnList { get => btnList;
            set { btnList = value; OnPropertyChanged("BtnList"); } }

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
            handler?.Invoke(null, new PropertyChangedEventArgs(info));
        }

        #endregion events
    }
}