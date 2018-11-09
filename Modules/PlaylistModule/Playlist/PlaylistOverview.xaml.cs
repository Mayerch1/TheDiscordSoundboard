using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DataManagement;

namespace PlaylistModule.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistOverview.xaml
    /// </summary>
    public partial class PlaylistOverview : UserControl, INotifyPropertyChanged
    {
        public delegate void OpenPlaylistHandler(int listId);

        public OpenPlaylistHandler OpenPlaylist;

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

        public PlaylistOverview(RuntimeData dt)
        {
            Data = dt;
            InitializeComponent();
        }

        private void btn_playlistAdd_Click(object sender, RoutedEventArgs e)
        {
            Util.IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new PlaylistAddPopup(Application.Current.MainWindow);
            popup.IsOpen = true;

            popup.Closed += delegate (object dSender, System.EventArgs pE)
            {
                Util.IO.BlurEffectManager.ToggleBlurEffect(false);

                if (popup.Result == true)
                    Data.Playlists.Add(new DataManagement.Playlist(popup.PlaylistName, popup.ImagePath));
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }

#pragma warning restore CS1591
}