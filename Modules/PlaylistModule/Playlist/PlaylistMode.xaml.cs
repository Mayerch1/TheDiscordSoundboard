using System.ComponentModel;
using System.Windows.Controls;
using DataManagement;

namespace PlaylistModule.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistMode.xaml
    /// </summary>
    public partial class PlaylistMode : UserControl, INotifyPropertyChanged
    {
        private RuntimeData data;

        public RuntimeData Data
        {
            get => data;
            set
            {
                data = value;
                OnPropertyChanged("Data");
            }
        }


        public PlaylistMode(RuntimeData dt)
        {
            Data = dt;
            InitializeComponent();

            PlaylistOverview playlistOverview = new PlaylistOverview(Data);
            playlistOverview.OpenPlaylist += openPlaylist;

            PlaylistGrid.Children.Add(playlistOverview);

            this.DataContext = Data;
           
        }

        public delegate void PlaylistItemEnqueuedHandler(DataManagement.FileData file);

        public PlaylistItemEnqueuedHandler PlaylistItemEnqueued;

        public delegate void PlaylistStartPlayHandler(int listIndex, uint fileIndex);

        public PlaylistStartPlayHandler PlaylistStartPlay;

        private void openPlaylist(int listId)
        {
            //change embeds for maingrit
            PlaylistGrid.Children.RemoveAt(0);

            PlaylistSingleView playList;
            if (listId >= 0)
                playList = new PlaylistSingleView((uint)listId, Data);
            else
                playList = new PlaylistSingleView(Data.History, Data);

            playList.SinglePlaylistStartPlay += InnerPlaylistPlay;
            playList.SinglePlaylistItemEnqueued += PlaylistItemQueued;
            playList.LeaveSingleView += LeaveSinglePlaylistView;

            PlaylistGrid.Children.Add(playList);
        }

        private void LeaveSinglePlaylistView()
        {
            PlaylistGrid.Children.RemoveAt(0);
            var overview = new PlaylistOverview(Data);
            overview.OpenPlaylist += openPlaylist;

            PlaylistGrid.Children.Add(overview);
        }

        private void PlaylistItemQueued(DataManagement.FileData file)
        {
            PlaylistItemEnqueued(file);
        }

        private void InnerPlaylistPlay(int listIndex, uint fileIndex)
        {
            PlaylistStartPlay(listIndex, fileIndex);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }

#pragma warning restore CS1591
}