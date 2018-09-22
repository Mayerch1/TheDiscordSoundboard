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
            var location = new Point(Application.Current.MainWindow.Left, Application.Current.MainWindow.Top);

            var dialog = new UI.Playlist.PlaylistAddDialog(location.X, location.Y, Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);

            var result = dialog.ShowDialog();

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