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
        private List<List<SocketVoiceChannel>> CompleteList { get; set; }

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
            CompleteList = await Handle.Bot.getAllChannels();
            //prevent crash, when not connected
            if (CompleteList != null)
            {
                //sort channels per server
                for (int i = 0; i < CompleteList.Count; i++)
                {
                    CompleteList[i] = CompleteList[i].OrderBy(o => o.Position).ToList();
                }

                populateSelector();
            }
        }

        private void populateSelector()
        {
            foreach (var server in CompleteList)
            {
                serverSelector.Items.Add(server[0].Guild);
            }
            //TODO: select server, which was selected last time
            //serverSelector.SelectedIndex = Handle.Data.Persistent.SelectedServerIndex;
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

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = (ComboBox)sender;
            channelViewControl.ItemsSource = CompleteList[box.SelectedIndex];
            Handle.Data.Persistent.SelectedServerIndex = box.SelectedIndex;
        }
    }
}