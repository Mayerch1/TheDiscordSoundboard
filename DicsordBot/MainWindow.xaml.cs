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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region fields

        private bool isLoading;
        # endregion

        #region propertys

        private double LastVolume { get; set; }

        public double Volume
        {
            get { return (double)Handle.Volume * 100.0f; }
            set
            {
                if (value != Volume)
                {
                    LastVolume = Volume;
                    Handle.Volume = (float)value / 100.0f;
                    setVolumeIcon();
                    OnPropertyChanged("Volume");
                }
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set { if (value != isLoading) { isLoading = value; OnPropertyChanged("IsLoading"); } }
        }

        #endregion propertys

        public MainWindow()
        {
            //need this, so other tasks will wait
            Handle.Data.loadData();
            InitializeComponent();

            IsLoading = false;

            //Don't even try, it's already changed

            registerEvents();
            initAsync();

            setVolumeIcon();
            DataContext = this;

            initDelayedAsync();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            cleanUp();
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

        private async void initDelayedAsync()
        {
            //delay this method by 2,5 seconds
            await Task.Delay(2500);

            await Handle.BotData.updateAvatar();
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

        #region event stuff

        private async void btn_InstantButton_Clicked(int btnListIndex)
        {
            await Handle.Bot.enqueueAsync(Handle.Data.Persistent.BtnList[btnListIndex]);
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
        }

        private void registerEvents()
        {
            //subsribe to intsant button event
            //TODO: give buttonui a name, then uncomment
            //ButtonUI.InstantButtonClicked += btn_InstantButton_Clicked;

            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
                IsLoading = false;
            };
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