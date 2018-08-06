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
            ChannelList = new List<SocketVoiceChannel>();

            InitializeComponent();
            initAsync();
            channelViewControl.ItemsSource = ChannelList;
        }

        private async void initAsync()
        {
            //get all channels from all servers
            var list = await Handle.Bot.getAllChannels();
            foreach (var element in list)
            {
                //sort channels by Position in Dc
                var sortedElement = element.OrderBy(o => o.Position).ToList();
                sortedElement.Add(null);
                ChannelList.AddRange(sortedElement);
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button btn = (Button)sender;
                Handle.ChannelId = (ulong)btn.Tag;
            }
        }

        private void userJoin_Click(object sender, RoutedEventArgs e)
        {
            Handle.ChannelId = 0;
        }
    }
}