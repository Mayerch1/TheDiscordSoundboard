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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    //TODO: insert button event for btn_loop_click

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region enums

        public enum LoopState { LoopNone, LoopOne, LoopAll, NextMode };

        #endregion enums

        #region fields

        private bool isLoading = false;
        private bool isEarrape = false;
        private LoopState loopStatus = LoopState.LoopNone;
        # endregion

        #region propertys

        private double LastVolume { get; set; }

        private double Volume
        {
            get { return (double)Handle.Volume * (100.0f * (1 / Handle.Data.Persistent.VolumeCap)); }
            set
            {
                if (value != Volume)
                {
                    LastVolume = Volume;
                    Handle.Volume = (float)value / (100.0f * (1 / Handle.Data.Persistent.VolumeCap));
                    setVolumeIcon();
                    OnPropertyChanged("Volume");
                }
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

        public double TitleTime
        {
            get { return Handle.Bot.CurrentTime.TotalSeconds; }
            set { Handle.Bot.skipToTime(TimeSpan.FromSeconds(value)); }
        }

        public double TotalTime
        {
            get { return Handle.Bot.TitleLenght.TotalSeconds; }
        }

        public string TotalTimeString { get { return TimeSpan.FromSeconds(TotalTime).ToString(@"mm\:ss"); } }
        public string TitleTimeString { get { return TimeSpan.FromSeconds(TitleTime).ToString(@"mm\:ss"); } }

        public bool IsLoading
        {
            get { return isLoading; }
            set { if (value != isLoading) { isLoading = value; OnPropertyChanged("IsLoading"); } }
        }

        public string ClientAvatar
        {
            get { return Handle.Data.Persistent.ClientAvatar; }
        }

        #endregion propertys

        public MainWindow()
        {
            //need this, so other tasks will wait
            Handle.Data.loadData();
            InitializeComponent();

            registerEvents();
            initAsync();

            initTimer();

            setVolumeIcon();
            setLoopStatus(LoopState.LoopNone);
            DataContext = this;

            initDelayedAsync();
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

            var client = Handle.BotData.extractClient(await Handle.Bot.getAllClients(), Handle.ClientId);
            Handle.BotData.updateAvatar(client);
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

        private void setLoopStatus(LoopState nextState = LoopState.NextMode)
        {
            //rotate through loop status
            if (nextState == LoopState.NextMode)
            {
                if (LoopStatus == LoopState.LoopNone)

                    nextState = LoopState.LoopOne;
                else if (LoopStatus == LoopState.LoopOne)

                    nextState = LoopState.LoopAll;
                else if (LoopStatus == LoopState.LoopAll)

                    nextState = LoopState.LoopNone;
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

            //TODO: set loop icon here

            LoopStatus = nextState;
        }

        private async void playClicked()
        {
            //toogle stream state, if stream is not empty
            IsLoading = true;
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
            if (isEarrape)
            {
                Volume = 100;
                //go around Volume calculations and slider visualisation
                Handle.Bot.Volume = Data.PersistentData.earrapeValue;
            }
            else
            {
                //make sure earrape cannot be accessed by undo volume changes
                Volume = LastVolume;
            }
        }

        #region event stuff

        private void registerEvents()
        {
            //subsribe to intsant button event

            ButtonUI.InstantButtonClicked += btn_InstantButton_Clicked;

            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
                IsLoading = false;
            };

            Handle.Bot.EarrapeStateChanged += delegate (bool isEarrape)
            {
                earrapeStatusChanged(isEarrape);
            };
            Handle.Bot.LoopStateChanged += delegate (bool isLoop)
            {
                if (isLoop)
                    setLoopStatus(LoopState.LoopOne);
                else
                    setLoopStatus(LoopState.LoopNone);
            };
        }

        private async void btn_InstantButton_Clicked(int btnListIndex)
        {
            await Handle.Bot.enqueueAsync(Handle.Data.Persistent.BtnList[btnListIndex]);
            //IDEA: maybe skip, or interrupt current stream
            if (!Handle.Bot.IsStreaming)
                await Handle.Bot.resumeStream();
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            playClicked();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            Handle.Bot.skipTrack();
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            //FUTURE: playlist: if time is below 2 seconds skip, else move to 0
            Handle.Bot.skipToTime(TimeSpan.Zero);
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
    }
}