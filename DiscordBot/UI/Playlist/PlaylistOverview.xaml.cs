using System.Windows;
using System.Windows.Controls;

namespace DiscordBot.UI.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistOverview.xaml
    /// </summary>
    public partial class PlaylistOverview : UserControl
    {
        public delegate void OpenPlaylistHandler(int listId);

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
                    Handle.Data.Playlists.Add(new DataManagement.Playlist(popup.PlaylistName, popup.ImagePath));
            };
        }

        private void btn_playlistOpen_Click(object sender, RoutedEventArgs e)
        {
            uint listId = (uint)((FrameworkElement)sender).Tag;
            OpenPlaylist((int)listId);
        }

        private void btn_showHistory_Click(object sender, RoutedEventArgs e)
        {
            OpenPlaylist(-1);
        }
    }

#pragma warning restore CS1591
}