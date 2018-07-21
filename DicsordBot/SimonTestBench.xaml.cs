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
        }

        private async void initAsync()
        {
            await Handle.Bot.connectToServerAsync();
            Console.WriteLine("Connected");
        }

        private async void Test()
        {
            await Handle.Bot.disconnectFromChannelAsync();
            IsLoading = true;

            if (Handle.Bot.IsStreaming)
            {
                Console.WriteLine("Stopping stream");
                await Handle.Bot.stopStreamAsync();
                Console.WriteLine("Stream stopped");
            }
            else
            {
                Console.WriteLine("Start connecting to voice");
                //await Handle.Bot.connectToChannelAsync(375065071946039297);
                await Handle.Bot.connectToChannelAsync();
                Console.WriteLine("Voice connected");
                await Handle.Bot.enqueueAsync(new Data.ButtonData
                {
                    File = @"F:\Christian\Music\Soundboard\Airporn.mp3",
                });
            }
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }

        private void btn_Instant_Click(object sender, RoutedEventArgs e)
        {
        }

        #region event stuff

        private void registerEvents()
        {
            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
                IsLoading = true;
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

        #endregion event stuff
    }
}