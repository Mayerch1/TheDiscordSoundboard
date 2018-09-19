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
            this.DataContext = Handle.Data;
        }

        public delegate void PlaylistItemEnqueuedHandler(Data.FileData file);

        public PlaylistItemEnqueuedHandler PlaylistItemEnqueued;

        public delegate void PlaylistStartPlayHandler(uint listIndex, uint fileTag);

        public PlaylistStartPlayHandler PlaylistStartPlay;

        private void btn_playlistAdd_Click(object sender, RoutedEventArgs e)
        {
            var location = this.PointToScreen(new Point(0, 0));
            var dialog = new PlaylistAddDialog(location.X, location.Y, this.ActualWidth, this.ActualHeight);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                Handle.Data.Playlists.Add(new Data.Playlist(dialog.PlaylistName));
            }
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            //opens new playlist as single view
            uint index = (uint)((FrameworkElement)sender).Tag;

            //change embeds for maingrit
            PlaylistGrid.Children.RemoveAt(0);
            var playList = new PlaylistSingleView(index);

            playList.SinglePlaylistStartPlay += InnerPlaylistPlay;
            playList.SinglePlaylistItemEnqueued += PlaylistItemQueued;

            PlaylistGrid.Children.Add(playList);
        }

        private void PlaylistItemQueued(Data.FileData file)
        {
            PlaylistItemEnqueued(file);
        }

        private void InnerPlaylistPlay(uint listIndex, uint FileTag)
        {
            PlaylistStartPlay(listIndex, FileTag);
        }
    }

#pragma warning restore CS1591
}