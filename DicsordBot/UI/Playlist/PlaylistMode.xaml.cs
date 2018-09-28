using System.Windows.Controls;

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

        public delegate void PlaylistStartPlayHandler(uint listIndex, uint fileIndex);

        public PlaylistStartPlayHandler PlaylistStartPlay;

        private void openPlaylist(uint listId)
        {
            //change embeds for maingrit
            PlaylistGrid.Children.RemoveAt(0);
            var playList = new PlaylistSingleView(listId);

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

        private void InnerPlaylistPlay(uint listIndex, uint fileIndex)
        {
            PlaylistStartPlay(listIndex, fileIndex);
        }
    }

#pragma warning restore CS1591
}