using Discord.WebSocket;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ChannelTree.xaml
    /// </summary>
    public partial class ChannelTree : UserControl
    {
        public List<SocketVoiceChannel> ChannelList { get; set; }

        public ChannelTree()
        {
            InitializeComponent();
            initAsync();
            //channelTree.ItemsSource = ChannelList;
            treeView.ItemsSource = ChannelList;
        }

        private async void initAsync()
        {
            var list = await Handle.Bot.getAllChannels();
            ChannelList = list[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}