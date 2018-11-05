using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

//TODO: rename project folder after merge
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


        private LoopState loopStatus = LoopState.LoopNone;
        private SnackbarMessageQueue snackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3500));
        private DispatcherTimer resizeTimer = new DispatcherTimer();

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


        private bool IsChannelListOpened { get; set; } = false;

        #endregion propertys

        public MainWindow()
        {
            //test comment
            //need this, so other tasks will wait
            Handle.Data.loadAll();

            InitializeComponent();

            LastVolume = Volume;

            //events
            registerEvents();
            registerEmbedEvents(ButtonUI);

            //file watcher
            IO.FileWatcher.StartMonitor(Handle.Data.Persistent.MediaSources);

            initTimer();

            //ui
            setVolumeIcon();
            btn_Repeat.Content = FindResource("IconRepeatOff");


            initHotkeys();

            //first startup sequence
            if (Handle.Data.Persistent.IsFirstStart)
            {
                Handle.Data.Persistent.IsFirstStart = false;

                btn_Settings_Click(null, null);
            }
            else
            {
                initAsync();
                initDelayedAsync();
            }

            IO.FileWatcher.indexFiles(Handle.Data.Persistent.MediaSources);
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
            Handle.Data.saveAll();

            IO.ImageManager.clearImageCache(Handle.Data.Playlists);
            IO.YTManager.clearVideoCache();

            //this will prevent the StreamState-changed handler from queueing the next song, when trying to disconnect
            Handle.Data.IsPlaylistPlaying = false;
            await Handle.Bot.disconnectFromServerAsync();
        }

        private async void initAsync()
        {
            Handle.Token = Handle.Data.Persistent.Token;
            Handle.ClientId = Handle.Data.Persistent.ClientId;
            Handle.Volume = Handle.Data.Persistent.Volume;
            Handle.ChannelId = Handle.Data.Persistent.ChannelId;

            await Handle.Bot.connectToServerAsync();
            if (await Misc.UpdateChecker.CheckForUpdate())
            {
                Handle.SnackbarWarning("A newer version is available", Handle.SnackbarAction.Update);
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
            //-------------------
            //timer for resizing
            resizeTimer.Tick += new EventHandler(Window_ResizingDone);
            resizeTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
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

            //resolve user to get avatar-url
            var client = Handle.BotMisc.extractClient(await Handle.Bot.getAllClients(true), Handle.ClientId);
            Handle.BotMisc.updateAvatar(client);
            OnPropertyChanged("ClientAvatar");

            //get channel list to display in TreeView
            initChannelList();
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
            if (Volume == 0)
            {
                btn_Volume.Content = FindResource("IconMuteVolume");
            }
            else if (Volume > 0 && Volume < 10)
            {
                btn_Volume.Content = FindResource("IconLowVolume");
            }
            else if (Volume >= 10 && Volume < 44)
            {
                btn_Volume.Content = FindResource("IconMediumVolume");
            }
            else
            {
                btn_Volume.Content = FindResource("IconHighVolume");
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

            //set icon, based on loopstate
            if (nextState == LoopState.LoopAll)
                btn_Repeat.Content = FindResource("IconRepeatAll");
            else if (nextState == LoopState.LoopOne && IsLoopForcedByBot)
            {
                //show different icon for bot override
                btn_Repeat.Content = FindResource("IconRepeatOnce");
            }
            else if (nextState == LoopState.LoopOne)
                btn_Repeat.Content = FindResource("IconRepeatOnce");
            else if (nextState == LoopState.LoopNone)
                btn_Repeat.Content = FindResource("IconRepeatOff");

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

        private void registerEmbedEvents(object embed)
        {
            switch (embed)
            {
                case UI.ButtonUI ui:
                    ui.InstantButtonClicked += btn_InstantButton_Clicked;
                    ui.ToggleHotkey += ToggleHotkey;
                    break;

                case UI.SearchMode ui:
                    ui.ListItemPlay += List_Item_Play;
                    break;

                case UI.Playlist.PlaylistMode ui:
                    ui.PlaylistStartPlay += Playlist_Play;
                    ui.PlaylistItemEnqueued += Playlist_SingleFile_Play;
                    break;

                case UI.StreamMode ui:
                    ui.PlayVideo += Stream_Video_Play;
                    ui.QueueVideo += Stream_Video_Queue;
                    ui.EulaRejected += Stream_Eula_Rejected;
                    break;
            }
        }

        //call only once
        private void registerEvents()
        {
            //event to resolve new clientName into clientId
            Handle.Data.Persistent.ClientNameChanged += Handle.ClientName_Changed;

            //universal SnackbarWarning
            Handle.SnackbarWarning += SnackbarMessage_Show;

            //route bot warning to universal Handle.SnackbarWarning
            Handle.Bot.SnackbarWarning += Handle.PassBotSnackbarWarning;


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
            IO.BlurEffectManager.ToggleBlurEffect += delegate(bool isEnabled)
            {
                IO.BlurEffectManager.ApplyBlurEffect(isEnabled, this);
            };
        }

        private void PassBlurEffectDelegate(bool isEnabled)
        { }

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
            Console.WriteLine("Hotkey pressed: " + lParam.ToString("x"));

            //spereate keyCodes from lParam
            uint keyCode = (((uint) lParam >> 16) & 0xFFFF);
            uint modCode = (uint) lParam & 0x00FFFF;

            //find button and trigger replay
            foreach (var hotkey in Handle.Data.Persistent.HotkeyList)
            {
                if (keyCode == hotkey.vk_code && modCode == hotkey.mod_code && hotkey.btn_id >= 0)
                    btn_InstantButton_Clicked(hotkey.btn_id);
            }
        }

        private void snackBar_Click()
        {
            //this is called, when no action is required/provided
        }

        private void SnackbarMessage_Show(string msg, Handle.SnackbarAction action)
        {
            string optionMsg = action.ToString();

            if (action == Handle.SnackbarAction.None)
                optionMsg = "Roger Dodger";

            Action handler;

            switch (action)
            {
                case Handle.SnackbarAction.Settings:
                    handler = btn_Settings_Click;
                    break;

                case Handle.SnackbarAction.Update:
                    handler = Misc.UpdateChecker.OpenUpdatePage;
                    break;

                default:
                    handler = snackBar_Click;
                    break;
            }


            SnackbarMessageQueue.Enqueue(msg, optionMsg, handler);
        }

        #region BotPlayDelegates

        private void btn_InstantButton_Clicked(int btnListIndex)
        {
            //interrupt stream
            triggerBotInstantReplay(Handle.BotMisc.getBotData(Handle.Data.Persistent.BtnList[btnListIndex]));
        }

        private void List_Item_Play(uint index, bool isPriority = true)
        {
            //search for file with tag
            foreach (var file in Handle.Data.Files)
            {
                if (file.Id == index)
                {
                    //create ButtonData to feed to bot
                    Data.BotData data = new Data.BotData(file.Name, file.Path);

                    if (isPriority)
                        //interrupt current stream
                        triggerBotInstantReplay(data);
                    else

                        triggerBotQueueReplay(data);
                }
            }
        }

        private void Playlist_SingleFile_Play(Data.FileData file)
        {
            triggerBotQueueReplay(new Data.BotData(file.Name, file.Path));
        }

        private void Stream_Video_Play(Data.BotData data)
        {
            triggerBotInstantReplay(data, true);
        }

        private void Stream_Video_Queue(Data.BotData data)
        {
            triggerBotQueueReplay(data, true);
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
            {
                Handle.Data.IsPlaylistPlaying = false;
                await Handle.Bot.stopStreamAsync();
            }

            //init playlist Mgr
            Data.FileData nextFile = Misc.PlaylistManager.InitList(listIndex, (int) fileIndex);


            if (nextFile != null)
            {
                triggerBotInstantReplay(new Data.BotData(nextFile.Name,
                    nextFile.Path));
                Handle.Data.IsPlaylistPlaying = true;
            }
        }


        private async void triggerBotInstantReplay(Data.BotData data, bool disableHistory = false)
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

        private async void triggerBotQueueReplay(Data.BotData data, bool disableHistory = false)
        {
            await Handle.Bot.enqueueAsync(data);

            if (!disableHistory)
                addTitleToHistory(data);
            //only resume, if not streaming + not in pause mode
            if (!Handle.Bot.IsStreaming)
                await Handle.Bot.resumeStream();
        }

        #endregion BotPlayDelegates

        private void addTitleToHistory(Data.BotData title)
        {
            if (File.Exists(title.filePath))
                Handle.Data.History.addTitle(IO.FileWatcher.getAllFileInfo(title.filePath));
        }

        private async void bot_streamState_Changed(bool newState)
        {
            //display pause icon, if bot is streaming
            if (newState /* is playing */)
                btn_Play.Content = FindResource("IconPause");
            else
            {
                btn_Play.Content = FindResource("IconPlay");


                //take next title in playlist
                if (Handle.Data.IsPlaylistPlaying && !Handle.Bot.IsPause)
                {
                    //disable playlistmode to prevent multiple skips
                    Handle.Data.IsPlaylistPlaying = false;

                    if (Handle.Bot.IsStreaming)
                        await Handle.Bot.stopStreamAsync();


                    //set loop-status of playlist
                    bool isLoop = LoopStatus == LoopState.LoopAll;
                    Misc.PlaylistManager.SetLoopState(isLoop);


                    var nextFile = Misc.PlaylistManager.GetNextTrack();

                    if (nextFile != null)
                    {
                        //enqueue next file
                        triggerBotQueueReplay(new Data.BotData(nextFile.Name, nextFile.Path));

                        Handle.Data.IsPlaylistPlaying = true;
                    }

                    //playlist stays on off, if null is returned
                }
            }
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            playClicked();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            Handle.Bot.skipTrack();
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            //skip prev title in playlist, when in playlist-mode and <2s
            if (Handle.Data.IsPlaylistPlaying && Handle.Bot.CurrentTime.TotalSeconds < 2)
            {
                //move to back, than skip one forward, 
                //this will skip the current track and start at t-1
                Misc.PlaylistManager.SkipBackwards();
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

        private void btn_About_Click(object sender, RoutedEventArgs e)
        {
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
            MainGrid.Children.Clear();
            UI.Playlist.PlaylistMode playUI = new UI.Playlist.PlaylistMode();
            registerEmbedEvents(playUI);
            MainGrid.Children.Add(playUI);
        }

        private void btn_Stream_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            UI.StreamMode streamUI = new UI.StreamMode();
            registerEmbedEvents(streamUI);
            MainGrid.Children.Add(streamUI);
        }


        private void btn_Settings_Click()
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new UI.Settings());
        }

        private void btn_Sounds_Click(object sender, RoutedEventArgs e)
        {
            //change embeds for maingrid
            MainGrid.Children.Clear();
            UI.ButtonUI btnUI = new UI.ButtonUI();
            registerEmbedEvents(btnUI);
            MainGrid.Children.Add(btnUI);
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Clear();
            UI.SearchMode searchMode = new UI.SearchMode();
            registerEmbedEvents(searchMode);

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

                Handle.SnackbarWarning(snackMsg, Handle.SnackbarAction.None);
            }
            catch
            {
                /* do nothing if someting other than a channel is selected*/
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

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //collapse channel popup
            if (IsChannelListOpened && !grd_ChannelList.IsMouseOver && !btn_Avatar.IsMouseOver)
            {
                btn_Avatar_Click(null, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeTimer.Stop();
            resizeTimer.Start();
            MainGrid.Width = MainGrid.ActualWidth;
            MainGrid.Height = MainGrid.ActualHeight;
            MainGrid.VerticalAlignment = VerticalAlignment.Top;
            MainGrid.HorizontalAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// gets called when a specific time after last resizing
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Window_ResizingDone(object sender, EventArgs e)
        {
            resizeTimer.Stop();
            MainGrid.Width = Double.NaN;
            MainGrid.Height = Double.NaN;
            MainGrid.VerticalAlignment = VerticalAlignment.Stretch;
            MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        #endregion event stuff

#pragma warning restore CS1591
    }
}