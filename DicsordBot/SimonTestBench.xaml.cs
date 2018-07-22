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
    /// Interaction logic for SimonTestBench.xaml
    /// </summary>
    public partial class SimonTestBench : UserControl, INotifyPropertyChanged
    {
        #region fields

        private bool isLoading;
        # endregion

        #region propertys

        private double LastVolume { get; set; }

        public double Volume
        {
            get { return (double)Handle.Data.Persistent.Volume; }
            set
            {
                if (value != Volume)
                {
                    LastVolume = Volume;
                    Handle.Data.Persistent.Volume = (float)value;
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

        public SimonTestBench()
        {
            IsLoading = false;

            //Don't even try, it's already changed

            InitializeComponent();
            registerEvents();
            initAsync();

            setVolumeIcon();
            DataContext = this;
        }

        private async void initAsync()
        {
            await Handle.Bot.connectToServerAsync();
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

        //TODO: get nicen name
        private async void playClicked()
        {
            IsLoading = true;
            if (Handle.Bot.IsStreaming)
            {
                await Handle.Bot.stopStreamAsync();
            }
            else
            {
                await Handle.Bot.connectToChannelAsync();
                await Handle.Bot.enqueueAsync(new Data.ButtonData
                {
                    Name = "Nothing",
                    File = @"",
                });
            }
        }

        #region event stuff

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
            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
                IsLoading = false;
            };
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

        #endregion event stuff
    }
}