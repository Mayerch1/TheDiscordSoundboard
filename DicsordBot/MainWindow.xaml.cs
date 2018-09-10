using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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

        public bool IsLoopForced { get; set; } = false;

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
            Handle.Data.loadData();

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
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            cleanUp();
            Console.WriteLine("Last user-code was executed");
        }

        private async void cleanUp()
        {
            Handle.Data.saveData();

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
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
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
                //in case bot double forced loop
                IsLoopForced = false;
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
            else if (nextState == LoopState.LoopReset && IsLoopForced)
            {
                //reset loop mode

                nextState = LoopState.LoopNone;
            }
            else if (nextState == LoopState.LoopReset)
            {
                //nothing changes
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
            else if (nextState == LoopState.LoopOne && IsLoopForced)
            {
                //show different icon for bot override
                IsLoopForced = false;
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

        private void registerEmbedEvents(ButtonUI btnUI)
        {
            //subsribe to intsant button event

            btnUI.InstantButtonClicked += btn_InstantButton_Clicked;
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

            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(tree_channelList_ItemExpanded));

            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
                //display pause icon, if bot is streaming
                if (newState)
                    btn_Play.Content = FindResource("IconPause");
                else
                    btn_Play.Content = FindResource("IconPlay");
            };

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
                    IsLoopForced = true;
                    setLoopStatus(LoopState.LoopOne);
                }
                else
                {
                    setLoopStatus(LoopState.LoopReset);
                }
            };
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

        private async void btn_InstantButton_Clicked(int btnListIndex)
        {
            await Handle.Bot.enqueueAsync(Handle.Data.Persistent.BtnList[btnListIndex]);
            //IDEA: skip when playlist, don't skip when instant buttons
            if (!Handle.Bot.IsStreaming)
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
            Handle.Bot.skipToTime(TimeSpan.Zero);
        }

        private void btn_Repeat_Click(object sender, RoutedEventArgs e)
        {
            setLoopStatus(LoopState.LoopNext);
        }

        private void btn_Earrape_Click(object sender, RoutedEventArgs e)
        {
            IsEarrape = !IsEarrape;
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

        private void btn_Settings_Click()
        {
            MainGrid.Child = null;
            MainGrid.Child = new Settings();
        }

        private void btn_Sounds_Click(object sender, RoutedEventArgs e)
        {
            //change embeds for maingrit
            MainGrid.Child = null;
            ButtonUI btnUI = new ButtonUI();
            registerEmbedEvents(btnUI);
            MainGrid.Child = btnUI;
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
                //TODO: check for long delays
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

            //get first expanded element, to save it for restart
        }

        private void tree_channelList_ItemExpanded(object sender, RoutedEventArgs e)
        {
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