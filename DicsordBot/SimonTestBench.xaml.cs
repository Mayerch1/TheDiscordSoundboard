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

            //PUBLISH: remove token
            //Don't even try, it's already changed
            Handle.Data.persistent.Token = "";

            InitializeComponent();
            registerEvents();

            Handle.BotHandler.connectServer();
        }

        private async void Test()
        {
            IsLoading = true;

            if (Handle.Bot.IsStreaming)
            {
                await Handle.Bot.stopStreamAsync();
            }
            else
            {
                await Handle.BotHandler.connectChannel(282933462690693132);

                await Handle.Bot.enqueueAsync(new ButtonData
                {
                    File = @"C:\Users\simon\Music\Soundboard\Wir_werden_sie jagen.mp3",
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