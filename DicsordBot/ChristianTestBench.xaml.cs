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
    /// Interaction logic for ChristianTestBench.xaml
    /// </summary>
    public partial class ChristianTestBench : UserControl, INotifyPropertyChanged
    {
        public ChristianTestBench()
        {
            InitializeComponent();
            DataContext = this;
            Test();
        }

        public double Volume
        {
            get { return Handle.Bot.Volume * 100; }
            set { Handle.Bot.Volume = (float)(value / 100); OnPropertyChanged("Volume"); }
        }

        public async void Test()
        {
            await Handle.Bot.connectToServerAsync();

            await Task.Delay(1250);

            await Handle.Bot.connectToChannelAsync(375065071946039297);
            await Handle.Bot.disconnectFromChannelAsync();
            await Handle.Bot.connectToChannelAsync(375065071946039297);

            Data.ButtonData btn = new Data.ButtonData();
            //btn.File = "F:\\Christian\\Music\\Alles_nur_Spass_Mix.mp3";
            btn.File = "F:\\Christian\\Music\\Soundboard\\Airporn.mp3";

            Handle.Bot.Volume = 0.5f;
            Handle.Bot.IsLoop = true;

            await Handle.Bot.enqueueAsync(btn);

            Console.WriteLine("Finished here");

            await Handle.Bot.disconnectFromChannelAsync();
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
    }
}