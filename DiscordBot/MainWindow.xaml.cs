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
using DiscordBot.UI;
using DiscordBot.UI.Tutorial;
using GithubVersionChecker;
using MaterialDesignThemes.Wpf;
using StreamModule;
using PlaylistModule;
using PlaylistModule.Playlist;
using Util.IO;

namespace DiscordBot
{
    /// <inheritdoc cref="T:System.Windows.Window"/>
    ///  <summary>
    ///  Interaction logic for MainWindow.xaml
    ///  </summary>
    //blub
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
        private static bool _queueMutex = false;


        private LoopState loopStatus = LoopState.LoopNone;
        private SnackbarMessageQueue snackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3500));
        private string currentSongName = "";
        private double livePitch = 1.0f;

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

        public double Volume
        {
            get => ((double) Handle.Volume * 100) * (1 / (Handle.Data.Persistent.VolumeCap / 100.0f));
            set
            {
                LastVolume = Volume;
                Handle.Volume = ((float) value / 100) * (Handle.Data.Persistent.VolumeCap / 100.0f);
                setVolumeIcon();
                OnPropertyChanged("Volume");
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

            LastVolume = Volume;


            SetDynamicMenu();

            //events
            registerEvents();

            registerButtonEvents(ButtonUI);

            //file watcher
            FileWatcher.StartMonitor(Handle.Data.Persistent.MediaSources, Handle.Data);

            initTimer();

            //ui
            setVolumeIcon();

            if (btn_Repeat.Content is PackIcon ic)
                ic.Kind = PackIconKind.RepeatOff;

            LivePitch = Handle.Pitch;


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

                initAsync();
                initDelayedAsync();
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
            Handle.Data.IsPlaylistPlaying = false;
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
            const int version = 0;

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

                    //---------STEP 2/3-------
                    // add new Modules or new Functions here
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
                            default:
                                Util.IO.LogManager.LogException(null, "Main/MainWindow",
                                    "Wrong Module ID. Fix or delete\"ModuleStates.xml\"", true);
                                break;
                        }
                    }
                }
            }
        }

        private async void initAsync()
        {
            Handle.Token = Handle.Data.Persistent.Token;
            Handle.ClientId = Handle.Data.Persistent.ClientId;
            Handle.Volume = Handle.Data.Persistent.Volume;
            Handle.ChannelId = Handle.Data.Persistent.ChannelId;

            await Handle.Bot.connectToServerAsync();
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

        private async void initDelayedAsync()
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

        private async void initChannelList()
        {
            //receives channel list + displays it when menu is opened
            Misc.ChannelListManager channelMgr = new Misc.ChannelListManager();
            await channelMgr.initAsync();

            channelMgr.populateTree(tree_channelList);
        }

        private void setVolumeIcon()
        {
            if (btn_Volume.Content is PackIcon ic)
            {
                if (Volume == 0)
                {
                    ic.Kind = PackIconKind.VolumeMute;
                }
                else if (Volume > 0 && Volume < 10)
                {
                    ic.Kind = PackIconKind.VolumeLow;
                }
                else if (Volume >= 10 && Volume < 44)
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
                await Handle.Bot.stopStreamAsync();
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

        private void triggerMasterReplay(BotData data, bool isInstant, bool disableHistory = false,
            bool isDiscord = true)
        {
            if (!isDiscord)
                return;

            if (isInstant)
            {
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
            triggerMasterReplay(new DataManagement.BotData(Handle.Data.Persistent.BtnList[btnListIndex]), isInstant);
        }

        private void List_Item_Play(uint index, bool isPriority = true)
        {
            //search for file with tag
            foreach (var file in Handle.Data.Files)
            {
                if (file.Id == index)
                {
                    //create ButtonData to feed to bot
                    DataManagement.BotData data = new DataManagement.BotData(file.Name, file.Path);
                    triggerMasterReplay(data, isPriority);
                }
            }
        }

        private void Playlist_SingleFile_Play(DataManagement.FileData file)
        {
            triggerMasterReplay(new DataManagement.BotData(file.Name, file.Path), false);
        }

        private void Stream_Video_Play(DataManagement.BotData data)
        {
            triggerMasterReplay(data, true, true);
        }

        private void Stream_Video_Queue(DataManagement.BotData data)
        {
            triggerMasterReplay(data, false, true);
        }

        private void Stream_Eula_Rejected()
        {
            //return to button ui
            btn_Sounds_Click(null, null);
        }

        /// <param name="listIndex">unique id field of playlist</param>
        /// <param name="fileIndex">index in the array of all playList files</param>
        private async void Playlist_Play(int listIndex, uint fileIndex)
        {
            //stop streaming
            if (Handle.Bot.IsStreaming)            
                await Handle.Bot.stopStreamAsync();
            
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

            //get first file for replay
            BotQueue.QueueItem? nextFile = Handle.Data.Queue.getNextItem();

            if (nextFile.HasValue)
            {
                BotQueue.QueueItem item = nextFile.Value;
                //instant enqueue for first item in list
                triggerMasterReplay(item.botData, true,item.disableHistory);
            }
        }
    

        private async void triggerInstantReplay(DataManagement.BotData data, bool disableHistory = false)
        {
            await triggerBotInstantReplay(data, disableHistory);
        }


        [Obsolete("Use self implemented queue, as private queue of bot is hard to control")]
        private async void triggerQueueReplay(DataManagement.BotData data, bool disableHistory = false)
        {
            await triggerBotQueueReplay(data, disableHistory);
        }


        private async Task triggerBotInstantReplay(DataManagement.BotData data, bool disableHistory)
        {
            //place song in front of queue
            await Handle.Bot.enqueuePriorityAsync(data);
            if (!disableHistory)
                addTitleToHistory(data);
            //start or skip current track
            if (!Handle.Bot.IsStreaming)
                await Handle.Bot.resumeStream();
            else
                Handle.Bot.skipTrack();
        }

        [Obsolete("Use self implemented queue, as private queue of hard to control")]
        private async Task triggerBotQueueReplay(DataManagement.BotData data, bool disableHistory)
        {
            await Handle.Bot.enqueueAsync(data);

            if (!disableHistory)
                addTitleToHistory(data);
            //only resume, if not streaming + not in pause mode
            if (!Handle.Bot.IsStreaming)
                await Handle.Bot.resumeStream();
        }

        #endregion BotPlayDelegates

        private void addTitleToHistory(DataManagement.BotData title)
        {
            if (File.Exists(title.filePath))
                Handle.Data.History.addTitle(FileWatcher.getAllFileInfo(title.filePath),
                    Handle.Data.Persistent.MaxHistoryLen);
        }

        private async void bot_streamState_Changed(bool newState, string songName)
        {
            //display pause icon, if bot is streaming
            if (newState /* is playing */)
            {
                if (btn_Play.Content is PackIcon ic)
                    ic.Kind = PackIconKind.Pause;
                //test if set properly
                CurrentSongName = songName;
            }
            else
            {
                if (!_queueMutex)
                {
                    _queueMutex = true;
                    
                    if (btn_Play.Content is PackIcon ic)
                        ic.Kind = PackIconKind.Play;

                    //if bot is paused, no track shall be skipped
                    if (!Handle.Bot.IsPause)
                    {
                        var nextItem = Handle.Data.Queue.getNextItem(LoopStatus == LoopState.LoopAll);

                        //take next title in playlist
                        if (nextItem.HasValue)
                        {
                            //wait until bot flushed the stream
                            if (Handle.Bot.IsStreaming)
                                await Handle.Bot.stopStreamAsync();

                            //next file
                            var itemVal = nextItem.Value;

                            //play the next file
                            triggerMasterReplay(itemVal.botData, true, itemVal.disableHistory);
                        }
                    }

                    _queueMutex = false;
                }
            }
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            playClicked();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            //TODO skip back
            //skip to next track
            Handle.Bot.skipTrack();
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            //TODO skip back
            //skip prev title in playlist, when in playlist-mode and <2s
            if (Handle.Data.IsPlaylistPlaying && Handle.Bot.CurrentTime.TotalSeconds < 2)
            {
                //move back
                Handle.Data.Queue.skipBackTrack();
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
            if (Volume > 0)
            {
                Volume = 0;
            }
            else
            {
                Volume = LastVolume;
            }
        }

        private void btn_PitchReset_Click(object sender, RoutedEventArgs e)
        {
                LivePitch = (float)1.0f;
            Handle.Pitch = (float) 1.0f;
        }
        
        private void Slider_DelayedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //is NewValue is the same as LivePitch
            Handle.Pitch = (float) e.NewValue;
           
            Console.WriteLine(@"Set Pitch to " + Handle.Pitch);
        }


        private void btn_About_Click(object sender, RoutedEventArgs e)
        {
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
            if (File.Exists("PlaylistModule.dll"))
                LoadPlaylistMode();
            else
                SnackbarManager.SnackbarMessage("Module not installed");
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


        private void btn_Settings_Click()
        {
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
            //buttonUI is mandatory
            //change embeds for maingrid
            MainGrid.Children.Clear();
            UI.ButtonUI btnUI = new UI.ButtonUI();
            registerButtonEvents(btnUI);
            MainGrid.Children.Add(btnUI);
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
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
                DialogHost.CloseDialogCommand.Execute(null, null);
                e.Handled = false;
            }


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
                initAsync();
                initDelayedAsync();
                btn_Settings_Click();
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

        #endregion stuff related to dll
#pragma warning restore CS1591


       
    }
}