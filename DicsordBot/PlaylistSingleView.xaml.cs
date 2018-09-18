using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for PlaylistSingleView.xaml
    /// </summary>
    public partial class PlaylistSingleView : UserControl, INotifyPropertyChanged
    {
        public delegate void SinglePlaylistItemEnqueuedHandler(Data.FileData file);

        public SinglePlaylistItemEnqueuedHandler SinglePlaylistItemEnqueued;

        public delegate void SinglePlaylistStartPlayHandler(uint listIndex, uint fileTag);

        public SinglePlaylistStartPlayHandler SinglePlaylistStartPlay;

        public PlaylistSingleView(uint _index)
        {
            index = _index;
            InitializeComponent();
            this.DataContext = this;
        }

        private uint index = 0;
        public Data.Playlist Playlist { get { return Handle.Data.Playlists[(int)index]; } set { Handle.Data.Playlists[(int)index] = value; } }
        public ObservableCollection<Data.FileData> PlaylistFiles { get { return Playlist.Tracks; } set { Playlist.Tracks = value; OnPropertyChanged("PlaylistFiles"); } }

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //start to replay the complete list
            uint tag = (uint)((FrameworkElement)sender).Tag;
            SinglePlaylistStartPlay(index, tag);
        }

        private void menu_openContext_Click(object sender, RoutedEventArgs e)
        {
            //find grandparent to opent context
            var listElement = sender as FrameworkElement;
            if (listElement != null)
            {
                var parent = listElement.Parent as FrameworkElement;
                if (parent != null)
                {
                    var grandParent = parent.Parent as FrameworkElement;
                    if (grandParent != null)
                        grandParent.ContextMenu.IsOpen = true;
                }
            }
        }

        private void menu_addToQueue_Clicked(object sender, RoutedEventArgs e)
        {
            //enque clicked file
            uint tag = (uint)((FrameworkElement)sender).Tag;

            foreach (var file in PlaylistFiles)
            {
                if (file.Id == tag)
                    SinglePlaylistItemEnqueued(file);
            }
        }

        private void context_AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //This should crash
            //uint tag = (uint)((FrameworkElement)sender).Tag;

            //ListItemPlay(tag, false);
        }

        #region events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion events
    }

#pragma warning restore CS1591
}