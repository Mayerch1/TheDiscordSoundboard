using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
#pragma warning disable CS1591

        #region enums

        public enum LoopState { LoopNone, LoopOne, LoopAll, LoopNext, LoopReset };

        #endregion enums

        #region fields

        private bool isEarrape = false;

        private LoopState loopStatus = LoopState.LoopNone;

        # endregion

        #region propertys

        private double LastVolume { get; set; }

        public double Volume
        {
            get { return ((double)Handle.Volume * 100) * (1 / (Handle.Data.Persistent.VolumeCap / 100.0f)); }
            set
            {
                LastVolume = Volume;
                Handle.Volume = ((float)value / 100) * (Handle.Data.Persistent.VolumeCap / 100.0f);
                setVolumeIcon();
                OnPropertyChanged("Volume");
            }
        }

        public bool IsEarrape
        {
            get { return isEarrape; }
            set
            {
                isEarrape = value;
                earrapeStatusChanged(isEarrape);
                OnPropertyChanged("IsEarrape");
            }
        }

        public bool IsLoop
        {
            get { return Handle.Bot.IsLoop; }
            set
            {
                Handle.Bot.IsLoop = value;
                OnPropertyChanged("IsLoop");
            }
        }

        public LoopState LoopStatus
        {
            get { return loopStatus; }
            set
            {
                loopStatus = value;
                OnPropertyChanged("LoopStatus");
            }
        }

        public bool IsLoopForcedByBot { get; set; } = false;

        public double TitleTime
        {
            get { return Handle.Bot.CurrentTime.TotalSeconds; }
            set { if (value < TotalTime) Handle.Bot.skipToTime(TimeSpan.FromSeconds(value)); }
        }

        public double TotalTime
        {
            get { return Handle.Bot.TitleLenght.TotalSeconds; }
        }

        public string TotalTimeString { get { return TimeSpan.FromSeconds(TotalTime).ToString(@"mm\:ss"); } }
        public string TitleTimeString { get { return TimeSpan.FromSeconds(TitleTime).ToString(@"mm\:ss"); } }

        public string ClientAvatar
        {
            get { return Handle.Data.Persistent.ClientAvatar; }
        }

        private bool IsChannelListOpened { get; set; } = false;

        #endregion propertys

        public MainWindow()
        {
            //need this, so other tasks will wait
            Handle.Data.loadAll();

            InitializeComponent();

            LastVolume = Volume;

            //events
            registerEvents();
            registerEmbedEvents(ButtonUI);

            //file watcher
            FileWatcher.StartMonitor(Handle.Data.Persistent.MediaSources);

            initTimer();

            //ui
            setVolumeIcon();
            btn_Repeat.Content = FindResource("IconRepeatOff");

            DataContext = this;

            //init snackbar msg
            var msgQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3500));
            snackBar_Hint.MessageQueue = (msgQueue);

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

            FileWatcher.indexFiles(Handle.Data.Persistent.MediaSources);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            cleanUp();
            terminateHotkeys();
            Console.WriteLine("Last user-code was executed");
        }

        #region hotkeys

        private void initHotkeys()
        {
            HotkeyManager.initHotkeys(this);
            //register all saved hotkeys
            foreach (var hotkey in Handle.Data.Persistent.HotkeyList)
                HotkeyManager.RegisterHotKey(this, hotkey.mod_code, hotkey.vk_code);
        }

        private void terminateHotkeys()
        {
            HotkeyManager.terminateHotkeys(this);
        }

        #endregion hotkeys

        private async void cleanUp()
        {
            Handle.Data.saveAll();

            await Handle.Bot.disconnectFromServerAsync();
        }

        private async void initAsync()
        {
            Handle.Token = Handle.Data.Persistent.Token;
            Handle.ClientId = Handle.Data.Persistent.ClientId;
            Handle.Volume = Handle.Data.Persistent.Volume;
            Handle.ChannelId = Handle.Data.Persistent.ChannelId;

            await Handle.Bot.connectToServerAsync();
            if (await UpdateChecker.CheckForUpdate())
            {
                SnackBarWarning_Show("A newer version is available", Bot.BotHandle.SnackBarAction.Update);
            }
        }

        private void initTimer()
        {
            //init timer, that fires every second to display time-slider
            //ticks 4 times a second
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Start();
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
            var client = Handle.BotData.extractClient(await Handle.Bot.getAllClients(), Handle.ClientId);
            Handle.BotData.updateAvatar(client);
            OnPropertyChanged("ClientAvatar");

            //get channel list to display in TreeView
            initChannelList();
        }

        private async void initChannelList()
        {
            //receives channel list + displays it when menu is opened
            ChannelListManager channelMgr = new ChannelListManager();
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
            Type objType = embed.GetType();

            if (objType == typeof(ButtonUI))
            {
                ((ButtonUI)embed).InstantButtonClicked += btn_InstantButton_Clicked;
            }
            else if (objType == typeof(SearchMode))
            {
                ((SearchMode)embed).ListItemPlay += List_Item_Play;
            }
            else if (objType == typeof(PlaylistMode))
            {
                ((PlaylistMode)embed).PlaylistItemPlay += Playlist_Item_Play;
            }
        }

        //call only once
        private void registerEvents()
        {
            //event to resolve new clientName into clientId
            Handle.Data.Persistent.ClientNameChanged += Handle.ClientName_Changed;
            Handle.Bot.ChannelWarning += Handle.ChannelWarning_Show;
            Handle.Bot.TokenWarning += Handle.TokenWarning_Show;
            Handle.Bot.FileWarning += Handle.FileWarning_Show;
            Handle.Bot.ClientWarning += Handle.ClientWarning_Show;
            Handle.Bot.SnackbarWarning += SnackBarWarning_Show;
            HotkeyManager.RegisteredHotkeyPressed += Hotkey_Pressed;

            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(tree_channelList_ItemExpanded));

            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += bot_streamState_Changed;

            //earrape event
            Handle.Bot.EarrapeStateChanged += delegate (bool isEarrape)
            {
                earrapeStatusChanged(isEarrape);
            };
            //loop state event
            Handle.Bot.LoopStateChanged += delegate (bool isLoop)
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
        }

        private void Hotkey_Pressed(IntPtr lParam)
        {
            Console.WriteLine("Hotkey pressed: " + lParam.ToString("x"));

            //spereate keyCodes from lParam
            uint keyCode = ((uint)lParam & 0xFF0000) >> (4 * 4);
            uint modCode = (uint)lParam & 0x00FFFF;

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

        private void SnackBarWarning_Show(string msg, Bot.BotHandle.SnackBarAction action)
        {
            string optionMsg = action.ToString();
            if (action == Bot.BotHandle.SnackBarAction.None)
                optionMsg = "Roger Dodger";

            Action handler;

            switch (action)
            {
                case Bot.BotHandle.SnackBarAction.Settings:
                    handler = btn_Settings_Click;
                    break;

                case Bot.BotHandle.SnackBarAction.Update:
                    handler = UpdateChecker.OpenUpdatePage;
                    break;

                default:
                    handler = snackBar_Click;
                    break;
            }

            snackBar_Hint.MessageQueue.Enqueue(msg, optionMsg, handler);
        }

        private void btn_InstantButton_Clicked(int btnListIndex)
        {
            //interrupt stream
            triggerBotInstantReplay(Handle.Data.Persistent.BtnList[btnListIndex]);
        }

        private void List_Item_Play(uint index, bool isPriority = true)
        {
            //search for file with tag
            foreach (var file in Handle.Data.Files)
            {
                if (file.Id == index)
                {
                    //create ButtonData to feed to bot
                    Data.ButtonData data = new Data.ButtonData(file.Name, file.Path);

                    if (isPriority)
                        //interrupt current stream
                        triggerBotInstantReplay(data);
                    else

                        triggerBotQueueReplay(data);
                }
            }
        }

        private void Playlist_Item_Play(uint id, uint fileIndex = 0)
        {
            //search for playlist
            foreach (var playlist in Handle.Data.Playlists)
            {
                if (playlist.Id == id)
                {
                    if (fileIndex < playlist.Tracks.Count)
                    {
                        //add first button
                        triggerBotInstantReplay(new Data.ButtonData(playlist.Tracks[(int)fileIndex].Name, playlist.Tracks[(int)fileIndex].Path));
                        //set playlist properties
                        Handle.Data.PlaylistIndex = id;
                        Handle.Data.PlaylistFileIndex = fileIndex;
                        Handle.Data.IsPlaylistPlaying = true;
                    }
                }
            }
        }

        private async void triggerBotInstantReplay(Data.ButtonData data)
        {
            //place song in front of queue
            await Handle.Bot.enqueuePriorityAsync(data);

            //start or skip current track
            if (!Handle.Bot.IsStreaming)
                await Handle.Bot.resumeStream();
            else
                Handle.Bot.skipTrack();
        }

        private void bot_streamState_Changed(bool newState)
        {
            //display pause icon, if bot is streaming
            if (newState)
                btn_Play.Content = FindResource("IconPause");
            else
            {
                btn_Play.Content = FindResource("IconPlay");

                //take next title in playlist
                if (Handle.Data.IsPlaylistPlaying)
                {
                    foreach (var playlist in Handle.Data.Playlists)
                    {
                        if (playlist.Id == Handle.Data.PlaylistIndex)
                        {
                            //only if list is not finished
                            if (++Handle.Data.PlaylistFileIndex < playlist.Tracks.Count)
                            {
                                //play next track
                                int index = (int)Handle.Data.PlaylistFileIndex;
                                triggerBotQueueReplay(new Data.ButtonData(playlist.Tracks[index].Name, playlist.Tracks[index].Path));
                            }
                            else
                            {
                                if (LoopStatus == LoopState.LoopAll)
                                {
                                    Handle.Data.PlaylistFileIndex = 0;
                                    //call this function again, to equeue the playlist
                                    triggerBotQueueReplay(new Data.ButtonData(playlist.Tracks[0].Name, playlist.Tracks[0].Path));
                                }
                                else
                                    //set properties for finished playlist
                                    Handle.Data.IsPlaylistPlaying = false;
                            }
                        }
                    }
                }
            }
        }

        private async void triggerBotQueueReplay(Data.ButtonData data)
        {
            await Handle.Bot.enqueueAsync(data);

            //TODO: check fix, add title to queue when on pause mode
            //only resume, if not streaming + not in pause mode
            if (!Handle.Bot.IsStreaming && Handle.Bot.IsBufferEmpty)
                await Handle.Bot.resumeStream();
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
            //FUTURE: playlist: if time is below 2 seconds skip, else move to 0
            //TODO: test future implementation

            //skip prev title in playlist, when in playlist-mode and <2s
            if (Handle.Data.IsPlaylistPlaying && Handle.Bot.CurrentTime.TotalSeconds < 2)
            {
                if (Handle.Data.PlaylistFileIndex > 0)
                {
                    //move playlist reader, skip
                    Handle.Data.PlaylistFileIndex -= 2;
                    Handle.Bot.skipTrack();
                }
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
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
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
            MainGrid.Child = null;
            MainGrid.Child = new About();
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            //args are not needed, enables use for delegate events
            btn_Settings_Click();
        }

        private void btn_Playlist_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Child = null;
            PlaylistMode playUI = new PlaylistMode();
            registerEmbedEvents(playUI);
            MainGrid.Child = playUI;
        }

        private void btn_Settings_Click()
        {
            MainGrid.Child = null;
            MainGrid.Child = new Settings();
        }

        private void btn_Sounds_Click(object sender, RoutedEventArgs e)
        {
            //change embeds for maingrid
            MainGrid.Child = null;
            ButtonUI btnUI = new ButtonUI();
            registerEmbedEvents(btnUI);
            MainGrid.Child = btnUI;
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Child = null;
            SearchMode searchMode = new SearchMode();
            registerEmbedEvents(searchMode);

            MainGrid.Child = searchMode;
        }

        private void btn_ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            //start storyboard from resources in xaml
            Storyboard sb;
            if (btn_ToggleMenu.IsChecked == true)
                sb = FindResource("OpenMenu") as Storyboard;
            else
                sb = FindResource("CloseMenu") as Storyboard;

            sb.Begin();
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

            sb.Begin();
        }

        private void tree_channelList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //get new selected tree
            try
            {
                var channel = (MyTreeItem)e.NewValue;
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
                SnackBarWarning_Show(snackMsg, Bot.BotHandle.SnackBarAction.None);
            }
            catch {/* do nothing if someting other than a channel is selected*/ }
        }

        private void tree_channelList_ItemExpanded(object sender, RoutedEventArgs e)
        {
            //get first expanded element, to save it for restart
            TreeViewItem treeItem = e.Source as TreeViewItem;

            if (treeItem.Tag != null)
                Handle.Data.Persistent.SelectedServerIndex = (int)treeItem.Tag;
        }

        private void scroll_channelList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //intercept scrollevent and make scrollviewer accept the wheel
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 10);
            e.Handled = true;
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
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event stuff

#pragma warning restore CS1591
    }
}