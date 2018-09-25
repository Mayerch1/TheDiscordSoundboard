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
            IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new PlaylistAddPopup(Application.Current.MainWindow);
            popup.IsOpen = true;

            popup.Closed += delegate (object dSender, System.EventArgs pE)
            {
                IO.BlurEffectManager.ToggleBlurEffect(false);

                if (popup.Result == true)
                    Handle.Data.Playlists.Add(new Data.Playlist(popup.PlaylistName));
            };
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            uint listId = (uint)((FrameworkElement)sender).Tag;
            OpenPlaylist(listId);
        }
    }

#pragma warning restore CS1591
}