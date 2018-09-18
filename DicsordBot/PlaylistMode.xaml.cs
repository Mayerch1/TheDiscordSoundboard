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
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistMode.xaml
    /// </summary>
    public partial class PlaylistMode : UserControl
    {
        public PlaylistMode()
        {
            InitializeComponent();
        }

        public delegate void PlaylistItemPlayHandler(uint tag, uint fileIndex);

        public PlaylistItemPlayHandler PlaylistItemPlay;

        private void btn_playlistAdd_Click(object sender, RoutedEventArgs e)
        {
            //TODO: playlist add dialog or dropdown
            var dialog = new PlaylistAddDialog();

            Window window = new Window
            {
                Title = "Create a new Playlist",
                Content = dialog,
            };

            var result = window.ShowDialog();
            if (result == true)
            {
                //TODO: add playlist
            }
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            //opens new playlist as single view
            uint index = (uint)((FrameworkElement)sender).Tag;

            //change embeds for maingrit
            PlaylistGrid.Children.RemoveAt(0);
            var playList = new PlaylistSingleView(index);

            PlaylistGrid.Children.Add(playList);
        }
    }

#pragma warning restore CS1591
}