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

            BlurEffectManager.ToggleBlurEffect(true);

            var result = dialog.ShowDialog();

            BlurEffectManager.ToggleBlurEffect(false);

            if (result == true)
            {
                Handle.Data.Playlists.Add(new Data.Playlist(dialog.PlaylistName));
            }
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            uint index = (uint)((FrameworkElement)sender).Tag;
            OpenPlaylist(index);
        }
    }

#pragma warning restore CS1591
}