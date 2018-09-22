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

namespace DicsordBot.UI.Playlist
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
            PlaylistOverview.OpenPlaylist += openPlaylist;
        }

        public delegate void PlaylistItemEnqueuedHandler(Data.FileData file);

        public PlaylistItemEnqueuedHandler PlaylistItemEnqueued;

        public delegate void PlaylistStartPlayHandler(uint listIndex, uint fileTag);

        public PlaylistStartPlayHandler PlaylistStartPlay;

        private void openPlaylist(uint listIndex)
        {
            //change embeds for maingrit
            PlaylistGrid.Children.RemoveAt(0);
            var playList = new PlaylistSingleView(listIndex);

            playList.SinglePlaylistStartPlay += InnerPlaylistPlay;
            playList.SinglePlaylistItemEnqueued += PlaylistItemQueued;
            playList.LeaveSingleView += LeaveSinglePlaylistView;

            PlaylistGrid.Children.Add(playList);
        }

        private void LeaveSinglePlaylistView()
        {
            PlaylistGrid.Children.RemoveAt(0);
            var overview = new PlaylistOverview();
            overview.OpenPlaylist += openPlaylist;

            PlaylistGrid.Children.Add(overview);
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