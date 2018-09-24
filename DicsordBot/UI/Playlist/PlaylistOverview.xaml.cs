using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DicsordBot.UI.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistOverview.xaml
    /// </summary>
    public partial class PlaylistOverview : UserControl
    {
        public delegate void OpenPlaylistHandler(uint listIndex);

        public OpenPlaylistHandler OpenPlaylist;

        public PlaylistOverview()
        {
            InitializeComponent();
        }

        private void btn_playlistAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UI.Playlist.PlaylistAddDialog(Application.Current.MainWindow);

            IO.BlurEffectManager.ToggleBlurEffect(true);

            dialog.Closing += delegate (object dSender, CancelEventArgs dE)
            {
                IO.BlurEffectManager.ToggleBlurEffect(false);

                if (dialog.Result == true)
                    Handle.Data.Playlists.Add(new Data.Playlist(dialog.PlaylistName));
            };

            dialog.Show();
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            uint listId = (uint)((FrameworkElement)sender).Tag;
            OpenPlaylist(listId);
        }
    }

#pragma warning restore CS1591
}