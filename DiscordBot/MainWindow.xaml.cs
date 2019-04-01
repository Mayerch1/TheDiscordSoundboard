using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DataManagement;
using DeviceStreamModule;
using DiscordBot.UI;
using DiscordBot.UI.Tutorial;
using GithubVersionChecker;
using MaterialDesignThemes.Wpf;
using StreamModule;
using PlaylistModule;
using PlaylistModule.Playlist;
using Util.com.chartlyrics.api;
using Util.IO;

namespace DiscordBot
{
    /// <inheritdoc cref="T:System.Windows.Window"/>
    ///  <summary>
    ///  Interaction logic for MainWindow.xaml
    ///  </summary>
    //blub2
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
#pragma warning disable CS1591

        #region enums

        public enum LoopState
        {
            LoopNone,
            LoopOne,
            LoopAll,
            LoopNext,
            LoopReset
        };

        #endregion enums

        #region fields

        private bool isEarrape = false;


        private LoopState loopStatus = LoopState.LoopNone;
        private SnackbarMessageQueue snackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3500));
        private string currentSongName = "";
        private double livePitch = 1.0f;
        private double liveVolume = 1.0f;

        #endregion fields

        #region propertys

        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get => snackbarMessageQueue;
            set
            {
                snackbarMessageQueue = value;
                OnPropertyChanged("SnackbarMessageQueue");
            }
        }

        public string CurrentSongName
        {
            get => currentSongName;
            set
            {
                currentSongName = value;
                OnPropertyChanged("CurrentSongName");
            }
        }


        private double LastVolume { get; set; }


        public double LiveVolume
        {
            get => liveVolume;
            set
            {
                liveVolume = value;
                setVolumeIcon(value);
                OnPropertyChanged("LiveVolume");
            }
        }

        public double LivePitch
        {
            get => livePitch;
            set
            {
                livePitch = value;
                OnPropertyChanged("LivePitch");
            }
        }

        public bool IsEarrape
        {
            get => isEarrape;
            set
            {
                isEarrape = value;
                earrapeStatusChanged(isEarrape);
                OnPropertyChanged("IsEarrape");
            }
        }


        public bool IsLoop
        {
            get => Handle.Bot.IsLoop;
            set
            {
                Handle.Bot.IsLoop = value;
                OnPropertyChanged("IsLoop");
            }
        }

        public LoopState LoopStatus
        {
            get => loopStatus;
            set
            {
                loopStatus = value;
                OnPropertyChanged("LoopStatus");
            }
        }

        public bool IsLoopForcedByBot { get; set; } = false;

        public double TitleTime
        {
            get => Handle.Bot.CurrentTime.TotalSeconds;
            set
            {
                if (value < TotalTime) Handle.Bot.skipToTime(TimeSpan.FromSeconds(value));
            }
        }

        public double TotalTime => Handle.Bot.TitleLenght.TotalSeconds;

        public string TotalTimeString => TimeSpan.FromSeconds(TotalTime).ToString(@"mm\:ss");

        public string TitleTimeString => TimeSpan.FromSeconds(TitleTime).ToString(@"mm\:ss");

        public string ClientAvatar => Handle.Data.Persistent.ClientAvatar;
        public string ClientName => Handle.Data.Persistent.ClientName;


        private bool IsChannelListOpened { get; set; } = false;

        #endregion propertys

        public MainWindow()
        {
            Util.IO.LogManager.InitLog();

            //load from properties, because library cannot access it
            try
            {
                Handle.Data.Persistent.SettingsPath = Properties.Settings.Default.Path;
            }
            catch (Exception ex)
            {
                Handle.Data.Persistent.SettingsPath = null;
                Util.IO.LogManager.LogException(ex, "DiscordBot/Main", "Could not load settings location", true);
            }


            //test comment
            //need this, so other tasks will wait
            Handle.Data.loadAll();

            //load/correct the Module data         
            assignHandleToModules();


            InitializeComponent();


            //--------
            //self assign theme values
            //this will apply themes to the ui, as it triggers delegates/events

            Handle.Data.Persistent.IsDarkTheme = Handle.Data.Persistent.IsDarkTheme;
            Handle.Data.Persistent.PrimarySwatch = Handle.Data.Persistent.PrimarySwatch;
            Handle.Data.Persistent.SecondarySwatch = Handle.Data.Persistent.SecondarySwatch;
            //--------


            SetDynamicMenu();

            //events
            registerEvents();

            registerButtonEvents(ButtonUI);

            //file watcher
            FileWatcher.StartMonitor(Handle.Data.Persistent.MediaSources, Handle.Data);

            initTimer();

            //ui
            LivePitch = Handle.Pitch;
            LiveVolume = ((double) Handle.Volume * 100) * (1 / (Handle.Data.Persistent.VolumeCap / 100.0f));
            LastVolume = LiveVolume;

            setVolumeIcon(LiveVolume);

            if (btn_Repeat.Content is PackIcon ic)
                ic.Kind = PackIconKind.RepeatOff;

            initHotkeys();

            //first startup sequence
            if (Handle.Data.Persistent.IsFirstStart)
            {
                Handle.Data.Persistent.IsFirstStart = false;

                //register events, this will trigger initialization after completed setup
                OpenTutorial();
            }
            else
            {
                foreach (var Module in Handle.Data.ModuleStates.Modules)
                {
                    foreach (var func in Module.Functions)
                    {
                        if (func.ID == Handle.Data.ModuleStates.AutostartId)
                        {
                            func.Handler?.Invoke(null, null);
                            break;
                        }
                    }
                }

                InitWrapperAsync();
            }

            FileWatcher.indexFiles(Handle.Data.Persistent.MediaSources);
        }




        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            cleanUp();
            terminateHotkeys();
        }

        #region hotkeys

        private void initHotkeys()
        {
            IO.HotkeyManager.initHotkeys(this);
            //register all saved hotkeys
            foreach (var hotkey in Handle.Data.Persistent.HotkeyList)
                IO.HotkeyManager.RegisterHotKey(this, hotkey.mod_code, hotkey.vk_code);
        }

        private void terminateHotkeys()
        {
            IO.HotkeyManager.terminateHotkeys(this);
        }

        #endregion hotkeys

        private async void InitWrapperAsync()
        {
            await initAsync();
            await initDelayedAsync();       
        }
        private async Task initAsync()
        {
            //refresh Binding for Button Width and Size
            Handle.Data.Persistent.BtnWidth = Handle.Data.Persistent.BtnWidth;
            Handle.Data.Persistent.BtnHeight = Handle.Data.Persistent.BtnHeight;

            Handle.Token = Handle.Data.Persistent.Token;
            Handle.ClientId = Handle.Data.Persistent.ClientId;
            Handle.Volume = Handle.Data.Persistent.Volume;
            Handle.ChannelId = Handle.Data.Persistent.ChannelId;

            await Handle.Bot.connectToServerAsync();
        }

        private async Task initDelayedAsync()
        {
            //delay this method by 2,5 seconds
            await Task.Delay(2500);

            //search for updates on github/releases
            var git = new GithubVersionChecker.GithubUpdateChecker(DataManagement.PersistentData.gitAuthor,
                DataManagement.PersistentData.gitRepo);
            if (await git.CheckForUpdateAsync(DataManagement.PersistentData.version, VersionChange.Revision))
            {
                SnackbarManager.SnackbarMessage("A newer version is available", SnackbarManager.SnackbarAction.Update);
            }


            Handle.ClientName_Changed(Handle.ClientName);

            //resolve user to get avatar-url
            var client = Handle.BotMisc.extractClient(await Handle.Bot.getAllClients(true), Handle.ClientId);
            Handle.BotMisc.updateAvatar(client);
            OnPropertyChanged("ClientAvatar");

            //get channel list to display in TreeView
            initChannelList();


            var palette = new PaletteHelper().QueryPalette();

            var x = new MaterialDesignColors.SwatchesProvider();
        }


        private async void cleanUp()
        {
            try
            {
                Properties.Settings.Default.Path = Handle.Data.Persistent.SettingsPath;
                Properties.Settings.Default.Save();
            }
            catch
            {
                /* do nothing */
            }

            if (!Handle.Data.Persistent.DontSave)
                Handle.Data.saveAll();

            ImageManager.clearImageCache(Handle.Data.Playlists);
            if (File.Exists("StreamModule.dll"))
                clearVideoCache();


            //this will prevent the StreamState-changed handler from queueing the next song, when trying to disconnect
            await Handle.Bot.disconnectFromServerAsync();
        }

        private void clearVideoCache()
        {
            //separate Function in case of missing dll
            StreamModule.YTManager.clearVideoCache();
        }


        private void assignHandleToModules()
        {
            //-------------------
            //IN CASE OF CHANGE:
            // for adding new Modules or Function
            // 3 Steps required
            // All Settings and Sidebars are managed by the program
            //--------------------

            //-------STEP 1/3-----
            // update the version number for the Modules
            // can be any higher integer number than the current
            //-------------------
            const int version = 1;

            //create entire class and default modules
            if (Handle.Data.ModuleStates == null || Handle.Data.ModuleStates.Version < version)
            {
                Handle.Data.ModuleStates = new DataManagement.ModuleManager();
                Handle.Data.ModuleStates.Version = version;

                Handle.Data.ModuleStates.Modules = new DataManagement.Module[]
                {
                    //mandatory
                    new Module(0, "Instant Buttons", "",
                        new DataManagement.Func[]
                        {
                            new DataManagement.Func(0, "SOUNDS", PackIconKind.ArrowRightDropCircleOutline)
                        }, true, true),

                    //optional Modules
                    new Module(2, "Playlist Module", "PlaylistModule.dll",
                        new DataManagement.Func[]
                        {
                            new DataManagement.Func(3, "SEARCH", PackIconKind.Magnify),
                            new DataManagement.Func(4, "PLAYLIST", PackIconKind.PlaylistPlay)
                        }),
                    new Module(3, "Stream Module", "StreamModule.dll",
                        new DataManagement.Func[]
                            {new DataManagement.Func(5, "STREAM", PackIconKind.YoutubePlay)}),

                    new Module(4, "Device Module", "DeviceStreamModule.dll",
                        new DataManagement.Func[]
                        {
                            new DataManagement.Func(6, "DEVICES", PackIconKind.Pipe),
                        }),
                    //---------STEP 2/3-------
                    // add new Modules or new Functions above
                    // ID should be unique to all functions in all Modules
                    // ModId should be unique to other modules, but can be the same as function id
                    // Not removable module has dll name "" (empty string)
                    //------------------------

                    //mandatory
                    new Module(1, "Settings", "",
                        new DataManagement.Func[]
                        {
                            new DataManagement.Func(1, "SETTINGS", PackIconKind.Settings),
                            new DataManagement.Func(2, "ABOUT", PackIconKind.Information)
                        }, true, true)
                };
            }


            //assign handle to functions in any case
            foreach (var Module in Handle.Data.ModuleStates.Modules)
            {
                foreach (var func in Module.Functions)
                {
                    //create new Handler, matching to corresponding function
                    if (func.Handler == null)
                    {
                        switch (func.ID)
                        {
                            //--------STEP 3/3---------
                            // add function handle for new Function
                            // func.ID represents id of the function
                            //-------------------------
                            case 0:
                                func.Handler = btn_Sounds_Click;
                                break;
                            case 1:
                                func.Handler = btn_Settings_Click;
                                break;
                            case 2:
                                func.Handler = btn_About_Click;
                                break;
                            case 3:
                                func.Handler = btn_Search_Click;
                                break;
                            case 4:
                                func.Handler = btn_Playlist_Click;
                                break;
                            case 5:
                                func.Handler = btn_Stream_Click;
                                break;
                            case 6:
                                func.Handler = btn_Device_Click;
                                break;
                            default:
                                Util.IO.LogManager.LogException(null, "Main/MainWindow",
                                    "Wrong Module ID. Fix or delete\"ModuleStates.xml\"", true);
                                break;
                        }
                    }
                }
            }
        }

       

        private void initTimer()
        {
            //init timer, that fires every second to display time-slider
            //ticks 4 times a second
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Start();
        }


        private void SetDynamicMenu()
        {
            //remove all buttons from stackPanel
            for (int i = (stackPanel_Menu.Children.Count - 1); i >= 0; i--)
            {
                //inverted for, because removing elements would change index of others
                if (stackPanel_Menu.Children[i] is Button btn)
                {
                    stackPanel_Menu.Children.RemoveAt(i);
                }
            }

            //ButtonMode is already added, as its mandatory
            var ButtonList = new List<Button>();


            //load all Modules marked in ModuleManager class
            foreach (var Module in Handle.Data.ModuleStates.Modules)
            {
                if (Module.IsModEnabled && (String.IsNullOrEmpty(Module.Dll) || File.Exists(Module.Dll)))
                {
                    foreach (var func in Module.Functions)
                    {
                        if (func.IsEnabled)
                        {
                            ButtonList.Add(getMenuButton(func.IconKind, func.Name, func.Handler));
                        }
                    }
                }
            }


            foreach (var item in ButtonList)
            {
                stackPanel_Menu.Children.Add(item);
            }
        }


        private Button getMenuButton(PackIconKind iconKind, string textStr, RoutedEventHandler handler)
        {
            var icon = new PackIcon();
            icon.Kind = iconKind;
            icon.Height = 35;
            icon.Width = 35;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;


            //create button (with Style) and StackPanel
            var btn = new Button()
            {
                Style = FindResource("MenuButton") as Style,
            };
            var stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            stack.Children.Add(icon);

            //create TextBlock with Style
            var menuTxt = new TextBlock()
            {
                Style = FindResource("MenuTextBlock") as Style,
            };
            menuTxt.Text = textStr;

            //add TextBlock, then add StackPanel to Button
            stack.Children.Add(menuTxt);

            btn.Content = stack;

            btn.Click += handler;

            return btn;
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //tell ui that positioning properties have changed
            if (Handle.Bot.IsStreaming)
            {
                OnPropertyChanged("TotalTime");
                OnPropertyChanged("TitleTime");
                OnPropertyChanged("TotalTimeString");
                OnPropertyChanged("TitleTimeString");
            }
        }

       
        private async void initChannelList()
        {
            //receives channel list + displays it when menu is opened
            Misc.ChannelListManager channelMgr = new Misc.ChannelListManager();
            await channelMgr.initAsync();

            channelMgr.populateTree(tree_channelList);
        }

        private void setVolumeIcon(double val)
        {
            if (btn_Volume.Content is PackIcon ic)
            {
                if (val == 0)
                {
                    ic.Kind = PackIconKind.VolumeMute;
                }
                else if (val > 0 && val < 10)
                {
                    ic.Kind = PackIconKind.VolumeLow;
                }
                else if (val >= 10 && val < 44)
                {
                    ic.Kind = PackIconKind.VolumeMedium;
                }
                else
                {
                    ic.Kind = PackIconKind.VolumeHigh;
                }
            }
        }

        private void setLoopStatus(LoopState nextState = LoopState.LoopNext)
        {
            //nothing changes
            if (nextState == LoopStatus)
            {
                //in case bot forced loop, but it's already set
                return;
            }

            //rotate through loop status
            if (nextState == LoopState.LoopNext)
            {
                if (LoopStatus == LoopState.LoopNone)

                    nextState = LoopState.LoopOne;
                else if (LoopStatus == LoopState.LoopOne)

                    nextState = LoopState.LoopAll;
                else if (LoopStatus == LoopState.LoopAll)

                    nextState = LoopState.LoopNone;
            }
            //if loop was forced by bot, reset
            else if (nextState == LoopState.LoopReset && IsLoopForcedByBot)
            {
                //reset loop mode
                nextState = LoopState.LoopNone;
                IsLoopForcedByBot = false;
            }
            else if (nextState == LoopState.LoopReset && !IsLoopForcedByBot)
            {
                //in case, user changed mode since bot override, nothing changes
                return;
            }

            //set loop-Status to bot
            if (nextState == LoopState.LoopOne)
            {
                Handle.Bot.IsLoop = true;
            }
            //unset loop to bot
            else
            {
                Handle.Bot.IsLoop = false;
            }


            if (btn_Repeat.Content is PackIcon ic)
            {
                //set icon, based on loopstate
                if (nextState == LoopState.LoopAll)
                    ic.Kind = PackIconKind.Repeat;
                else if (nextState == LoopState.LoopOne && IsLoopForcedByBot)
                {
                    //show different icon for bot override
                    ic.Kind = PackIconKind.RepeatOnce;
                }
                else if (nextState == LoopState.LoopOne)
                    ic.Kind = PackIconKind.RepeatOnce;
                else if (nextState == LoopState.LoopNone)
                    ic.Kind = PackIconKind.RepeatOff;
            }

            LoopStatus = nextState;
        }

        private async void playClicked()
        {
            //toogle stream state, if stream is not empty

            if (Handle.Bot.IsStreaming)
            {
                Handle.Bot.IsPause = true;
                await Handle.Bot.stopStreamAsync(false, false);
            }
            else if (!Handle.Bot.IsBufferEmpty)
            {
                await Handle.Bot.resumeStream();
            }
        }

        private void earrapeStatusChanged(bool isEarrape)
        {
            Handle.Bot.IsEarrape = isEarrape;
        }

        #region event stuff

        //call only once
        private void registerEvents()
        {
            //event to resolve new clientName into clientId
            Handle.Data.Persistent.ClientNameChanged += Handle.ClientName_Changed;

            //universal SnackbarWarning
            SnackbarManager.SnackbarMessage += SnackbarMessage_Show;


            //hotkey stuff
            IO.HotkeyManager.RegisteredHotkeyPressed += Hotkey_Pressed;

            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(tree_channelList_ItemExpanded));

            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += bot_streamState_Changed;

            //event Handler when bot finished current file
            Handle.Bot.EndOfFile += bot_EndOfFile_Trigger;

            //earrape event
            Handle.Bot.EarrapeStateChanged += delegate(bool isEarrape) { earrapeStatusChanged(isEarrape); };
            //loop state event
            Handle.Bot.LoopStateChanged += delegate(bool isLoop)
            {
                if (isLoop)
                {
                    IsLoopForcedByBot = true;
                    setLoopStatus(LoopState.LoopOne);
                }
                else
                {
                    //isLoopForcedByBot is reset in the method
                    setLoopStatus(LoopState.LoopReset);
                }
            };

            //fancy stuff
            Util.IO.BlurEffectManager.ToggleBlurEffect += delegate(bool isEnabled)
            {
                Util.IO.BlurEffectManager.ApplyBlurEffect(isEnabled, this);
            };
        }


        private void ToggleHotkey(bool isEnabled)
        {
            //enable/disable hotkeys
            if (isEnabled)
            {
                initHotkeys();
            }
            else
                terminateHotkeys();
        }

        private void Hotkey_Pressed(IntPtr lParam)
        {
            //Console.WriteLine("Hotkey pressed: " + lParam.ToString("x"));

            //spereate keyCodes from lParam
            uint keyCode = (((uint) lParam >> 16) & 0xFFFF);
            uint modCode = (uint) lParam & 0x00FFFF;

            //find button and trigger replay
            foreach (var hotkey in Handle.Data.Persistent.HotkeyList)
            {
                if (keyCode == hotkey.vk_code && modCode == hotkey.mod_code && hotkey.btn_id >= 0)
                    btn_InstantButton_Clicked(hotkey.btn_id, true);
            }
        }

        private void snackBar_Click()
        {
            //this is called, when no action is required/provided
        }

        private void snackBar_OpenUpdate()
        {
            System.Diagnostics.Process.Start(DataManagement.PersistentData.gitCompleteUrl);
        }


        private void SnackbarMessage_Show(string msg, SnackbarManager.SnackbarAction action)
        {
            string optionMsg = action.ToString();

            if (action == SnackbarManager.SnackbarAction.None)
                optionMsg = "Roger Dodger";

            Action handler;

            switch (action)
            {
                case SnackbarManager.SnackbarAction.Settings:
                    handler = btn_Settings_Click;
                    break;

                case SnackbarManager.SnackbarAction.Update:
                    handler = snackBar_OpenUpdate;
                    break;

                case SnackbarManager.SnackbarAction.Log:
                    handler = Util.IO.LogManager.OpenLog;
                    break;

                default:
                    handler = snackBar_Click;
                    break;
            }


            SnackbarMessageQueue.Enqueue(msg, optionMsg, handler);
        }

        #region BotPlayDelegates

        private void triggerMasterReplay(BotData data, bool isInstant, bool isPlaylist, bool disableHistory = false,
            bool isDiscord = true)
        {
            if (isInstant)
            {
                //only if not playlist
                if (!isPlaylist)
                    //instant play aborts the playlist
                    Handle.Data.Queue.clearPlaylist();

                //prepare lyrics w/o stressing the api
                Util.IO.LyricsManager.setParameter(data.name, data.author);

                //interrupt any stream
                triggerInstantReplay(data, disableHistory);
            }
            else
            {
                Handle.Data.Queue.enqueue(data, disableHistory);
            }
        }


        private void btn_InstantButton_Clicked(int btnListIndex, bool isInstant)
        {
            //especially isPlaylist argument
            triggerMasterReplay(new DataManagement.BotData(Handle.Data.Persistent.BtnList[btnListIndex]), isInstant,
                false);
        }

        private void List_Item_Play(uint index, bool isPriority = true)
        {
            //search for file with tag
            foreach (var file in Handle.Data.Files)
            {
                if (file.Id == index)
                {
                    //create ButtonData to feed to bot
                    DataManagement.BotData data = new DataManagement.BotData(file.Name, file.Path, "", "", file.Author);
                    triggerMasterReplay(data, isPriority, false);
                }
            }
        }

        private void Playlist_SingleFile_Play(DataManagement.FileData file)
        {
            triggerMasterReplay(new DataManagement.BotData(file.Name, file.Path, "", "", file.Author), false, false);
        }

        private void Stream_Video_Play(DataManagement.BotData data)
        {
            triggerMasterReplay(data, true, false, true);
        }

        private void Stream_Video_Queue(DataManagement.BotData data)
        {
            //queue play will NOT abort playlist
            triggerMasterReplay(data, false, false, true);
        }

        private void Stream_Eula_Rejected()
        {
            //return to button ui
            btn_Sounds_Click(null, null);
        }

        private void Device_StartStream(string name, string id)
        {
            //remove all elements because of infinite stream from device
            Handle.Data.Queue.clearQueue();

            var data = new BotData(name, "", "", id, "");
            triggerMasterReplay(data, true, false, true);
        }

       

        /// <param name="listIndex">unique id field of playlist</param>
        /// <param name="fileIndex">index in the array of all playList files</param>
        private async void Playlist_Play(int listIndex, uint fileIndex)
        {
            //delete the current list, keep single-item queues
            Handle.Data.Queue.clearPlaylist();

            //stop streaming
            if (Handle.Bot.IsStreaming)
                await Handle.Bot.stopStreamAsync(true, false);

            //add the list or history 
            Playlist list;
            bool isHistory = false;

            if (listIndex < 0)
            {
                list = Handle.Data.History;
                isHistory = true;
            }
            else
            {
                if (listIndex < Handle.Data.Playlists.Count)
                    list = Handle.Data.Playlists[listIndex];
                else
                    return;
            }

            Handle.Data.Queue.enqueuePlaylist(list, fileIndex, isHistory);

            //get next File from BotQueue
            bot_EndOfFile_Trigger();
        }


        private async void triggerInstantReplay(DataManagement.BotData data, bool disableHistory = false)
        {
            await triggerBotInstantReplay(data, disableHistory);
        }


        private async Task triggerBotInstantReplay(DataManagement.BotData data, bool disableHistory)
        {
            //place song in front of queue
            await Handle.Bot.loadFileAsync(data);

            if (!disableHistory)
                addTitleToHistory(data);
            //start or skip current track

            await Handle.Bot.resumeStream();
        }

        #endregion BotPlayDelegates


        private void btn_Disconnect_Clicked(object sender, RoutedEventArgs e)
        {
            DisconnectFromChannel();
        }

        private async void DisconnectFromChannel()
        {
            await Handle.Bot.disconnectFromChannelAsync();
        }


        private void btn_LyricShow_Click(object sender, RoutedEventArgs e)
        {
            if (LyricGrid.Visibility == Visibility.Visible)
            {
                LyricGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                LyricGrid.Visibility = Visibility.Visible;


                var lyrics = Util.IO.LyricsManager.getLyrics();

                if (lyrics != null)
                {
                    LyricsSheet.setTitle(lyrics.LyricSong);
                    LyricsSheet.setAuth(lyrics.LyricArtist);
                    LyricsSheet.setLyric(lyrics.Lyric);
                }
                else
                {
                    LyricsSheet.setTitle(CurrentSongName);
                    LyricsSheet.setAuth("");
                    LyricsSheet.setLyric("");
                }
            }
        }

        private void addTitleToHistory(DataManagement.BotData title)
        {
            if (File.Exists(title.filePath))
                Handle.Data.History.addTitle(FileWatcher.getAllFileInfo(title.filePath),
                    Handle.Data.Persistent.MaxHistoryLen);
        }


        private void bot_EndOfFile_Trigger()
        {
            //only triggered when bot ended file
            //NOT triggered on pause or loop
            //if (!_queueMutex)
            //{
            //_queueMutex = true;
            BotQueue.QueueItem? nextItem = Handle.Data.Queue.getNextItem(LoopStatus == LoopState.LoopAll);

            //take next title in playlist
            if (nextItem.HasValue)
            {
                //bot is not streaming when reaching this point

                //next file
                var itemVal = nextItem.Value;

                //play the next file
                triggerMasterReplay(itemVal.botData, true, true, itemVal.disableHistory);
            }

            //_queueMutex = false;
            //}
        }


        private void bot_streamState_Changed(bool newState, string songName)
        {
            CurrentSongName = songName;
            //display pause icon, if bot is streaming
            if (newState /* is playing */)
            {
                if (btn_Play.Content is PackIcon ic)
                    ic.Kind = PackIconKind.Pause;
                //test if set properly
            }
            else
            {
                if (btn_Play.Content is PackIcon ic)
                    ic.Kind = PackIconKind.Play;
            }
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            playClicked();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            //skip to next track
            Handle.Bot.skipTrack();
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            //skip prev title in playlist, when in playlist-mode and <2s
            if (Handle.Bot.CurrentTime.TotalSeconds < 2)
            {
                //move back
                Handle.Data.Queue.skipBackTrack(2);
                //skips one forward, so bot will get the next queued song
                Handle.Bot.skipTrack();
            }
            else
                //skip to beginning of track
                Handle.Bot.skipToTime(TimeSpan.Zero);
        }

        private void btn_Repeat_Click(object sender, RoutedEventArgs e)
        {
            IsLoopForcedByBot = false;
            setLoopStatus(LoopState.LoopNext);
        }

        private void btn_Earrape_Click(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton) sender).IsChecked == true)
                IsEarrape = true;
            else
                IsEarrape = false;
        }

        private void btn_Volume_Click(object sender, RoutedEventArgs e)
        {
            if (LiveVolume > 0)
            {
                LastVolume = LiveVolume;
                LiveVolume = 0;
            }
            else
            {
                LiveVolume = LastVolume;
            }
        }

        private void btn_PitchReset_Click(object sender, RoutedEventArgs e)
        {
            LivePitch = (float) 1.0f;
            Handle.Pitch = (float) 1.0f;
        }

        private void Slider_DelayedPitchChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //is NewValue is the same as LivePitch
            Handle.Pitch = (float) e.NewValue;

            Console.WriteLine(@"Set Pitch to " + Handle.Pitch);
        }

        private void Slider_DelayedVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Handle.Volume = ((float) e.NewValue / 100) * (Handle.Data.Persistent.VolumeCap / 100.0f);
            Console.WriteLine(@"Set Volume to " + Handle.Volume);
        }


        private void btn_About_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();
            //About is mandatory
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new UI.About());
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            //args are not needed, enables use for delegate events
            btn_Settings_Click();
        }

        private void btn_Playlist_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();

            if (File.Exists("PlaylistModule.dll"))
                LoadPlaylistMode();
            else
                SnackbarManager.SnackbarMessage("Module not installed");
        }


        /// <summary>
        /// Close menues,... for switching the tab/page
        /// </summary>
        private void SideChanged()
        {
            LyricGrid.Visibility = Visibility.Hidden;
        }

        private void LoadPlaylistMode()
        {
            MainGrid.Children.Clear();
            PlaylistModule.Playlist.PlaylistMode playUI = new PlaylistModule.Playlist.PlaylistMode(Handle.Data);
            registerPlaylistEvents(playUI);
            MainGrid.Children.Add(playUI);
        }

        private void btn_Stream_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();
            if (File.Exists("StreamModule.dll"))
                LoadStreamMode();
            else
                SnackbarManager.SnackbarMessage("Module not installed");
        }

        private void LoadStreamMode()
        {
            MainGrid.Children.Clear();
            //UI.StreamMode streamUI = new UI.StreamMode();

            StreamMode streamUI = new StreamMode(Handle.Data);
            registerStreamEvents(streamUI);
            MainGrid.Children.Add(streamUI);
        }

        private void btn_Device_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();
            if (File.Exists("DeviceStreamModule.dll"))
                LoadDeviceMode();
            else
                SnackbarManager.SnackbarMessage("Module not installed");
        }


        private void LoadDeviceMode()
        {
            MainGrid.Children.Clear();
            //UI.StreamMode streamUI = new UI.StreamMode();

            DeviceStreamModule.DeviceMode deviceUi = new DeviceStreamModule.DeviceMode();
            registerDeviceEvents(deviceUi);
            MainGrid.Children.Add(deviceUi);
        }


        private void btn_Settings_Click()
        {
            SideChanged();
            //settings are mandatory
            MainGrid.Children.Clear();
            var ui = new UI.Settings();

            //reload sidebar, when modules are checked/unchecked
            ui.RefreshModules += SetDynamicMenu;
            ui.OpenTutorial += OpenTutorial;

            MainGrid.Children.Add(ui);
        }

        private void OpenTutorial()
        {
            var ui = new TutorialMaster();
            registerTutorialEvents(ui);

            //start tutorial
            MainGrid.Children.Clear();
            MainGrid.Children.Add(ui);
        }

        private void btn_Sounds_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();
            //buttonUI is mandatory
            //change embeds for maingrid
            MainGrid.Children.Clear();
            UI.ButtonUI btnUI = new UI.ButtonUI();
            registerButtonEvents(btnUI);
            MainGrid.Children.Add(btnUI);
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            SideChanged();

            if (File.Exists("PlaylistModule.dll"))
                LoadSearchMode();
            else
                SnackbarManager.SnackbarMessage("Module not installed");
        }

        private void LoadSearchMode()
        {
            MainGrid.Children.Clear();
            SearchMode searchMode = new SearchMode(Handle.Data);
            registerSearchEvents(searchMode);

            MainGrid.Children.Add(searchMode);
        }

        private void btn_ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            //start storyboard from resources in xaml
            Storyboard sb;
            if (btn_ToggleMenu.IsChecked == true)
                sb = FindResource("OpenMenu") as Storyboard;
            else
                sb = FindResource("CloseMenu") as Storyboard;

            sb?.Begin();
        }

        private void btn_Avatar_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb;

            if (IsChannelListOpened)
            {
                sb = FindResource("CloseChannelList") as Storyboard;
                IsChannelListOpened = false;
            }
            else
            {
                sb = FindResource("OpenChannelList") as Storyboard;

                initChannelList();
                IsChannelListOpened = true;
            }

            sb?.Begin();
        }

        private void tree_channelList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //get new selected tree
            try
            {
                var channel = (Misc.MyTreeItem) e.NewValue;
                Handle.ChannelId = channel.id;

                //set new channel and temp/perm parameter
                string snackMsg = "";
                if (channel.id != 0)
                {
                    if (box_IsPermanentChannel.IsChecked == true)
                    {
                        Handle.Bot.IsTempChannelId = false;
                        snackMsg = "Using new channel permanently";
                    }
                    else
                    {
                        Handle.Bot.IsTempChannelId = true;
                        snackMsg = "Using new channel the next time";
                    }
                }
                else
                {
                    snackMsg = "Joining to owner permanently";
                }

                SnackbarManager.SnackbarMessage(snackMsg);
            }
            catch
            {
                /* do nothing if something other than a channel is selected*/
            }
        }

        private void tree_channelList_ItemExpanded(object sender, RoutedEventArgs e)
        {
            //get first expanded element, to save it for restart
            TreeViewItem treeItem = e.Source as TreeViewItem;

            if (treeItem?.Tag != null)
                Handle.Data.Persistent.SelectedServerIndex = (int) treeItem.Tag;
        }

        private void scroll_channelList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //intercept scrollevent and make scrollviewer accept the wheel
            ScrollViewer scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 10);
            e.Handled = true;
        }

        private void scroll_channelList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //prevent sideways scrolling
            if (e.HorizontalChange > 0)
            {
                ScrollViewer scv = (ScrollViewer) sender;
                scv.ScrollToHorizontalOffset(0);
            }
        }

        private void dialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //DO NOT REMOVE
            //this is needed, to call CloseDialogCommand.Execute(null, null) from code
            Console.WriteLine("SAMPLE 1: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));

            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //collapse channel popup
            if (IsChannelListOpened && !grd_ChannelList.IsMouseOver && !btn_Avatar.IsMouseOver)
            {
                btn_Avatar_Click(null, null);
            }

            //only close dialog, if click was outside of the dialog     
            if (!dialogHost_OtherControls.IsMouseOver)
            {
                if (dialogHost_OtherControls is DialogHost host)
                {
                    DialogHost.CloseDialogCommand.Execute(null, host);
                }
            }

            e.Handled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion event stuff


        #region stuff related to dll


       


        private void registerTutorialEvents(TutorialMaster ui)
        {
            ui.TutorialFinished += delegate
            {
                InitWrapperAsync();
                btn_Sounds_Click(null, null);
            };
        }

        private void registerButtonEvents(ButtonUI ui)
        {
            ui.InstantButtonClicked += btn_InstantButton_Clicked;
            ui.ToggleHotkey += ToggleHotkey;
        }

        private void registerSearchEvents(SearchMode ui)
        {
            ui.ListItemPlay += List_Item_Play;
        }

        private void registerPlaylistEvents(PlaylistMode ui)
        {
            ui.PlaylistStartPlay += Playlist_Play;
            ui.PlaylistItemEnqueued += Playlist_SingleFile_Play;
        }

        private void registerStreamEvents(StreamMode ui)
        {
            ui.PlayVideo += Stream_Video_Play;
            ui.QueueVideo += Stream_Video_Queue;
            ui.EulaRejected += Stream_Eula_Rejected;
        }

        private void registerDeviceEvents(DeviceMode ui)
        {
            ui.DeviceStartStream += Device_StartStream;
            //ui.DeviceStopStream += Device_StopStream;
            //TODO implement
        }

        #endregion stuff related to dll

#pragma warning restore CS1591
    }
}