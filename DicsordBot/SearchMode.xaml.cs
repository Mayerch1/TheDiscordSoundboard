using System;
using System.Collections;
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
    /// Interaction logic for SearchMode.xaml
    /// </summary>
    public partial class SearchMode : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Data.FileData> filteredFiles;
        public ObservableCollection<Data.FileData> FilteredFiles { get { return filteredFiles; } set { filteredFiles = value; OnPropertyChanged("FilteredFiles"); } }
        public ObservableCollection<Data.Playlist> Playlists { get { return Handle.Data.Playlists; } set { Handle.Data.Playlists = value; OnPropertyChanged("Playlists"); } }

        public delegate void ListItemPlayHandler(uint tag, bool isPriority);

        public ListItemPlayHandler ListItemPlay;

        public SearchMode()
        {
            //make deep copy
            FilteredFiles = new ObservableCollection<Data.FileData>(Handle.Data.Files);

            InitializeComponent();
            this.DataContext = this;
        }

        private bool checkContainLowerCase(Data.FileData file, string filter)
        {
            string filterLow = filter.ToLower();

            //filter for all known attributes (ignore case)
            if (file.Name.ToLower().Contains(filterLow) || file.Author.ToLower().Contains(filterLow)
                || file.Album.ToLower().Contains(filterLow) || file.Genre.ToLower().Contains(filterLow))
            {
                return true;
            }
            else
                return false;
        }

        private void filterListBox(string filter)
        {
            //clear list and apply filter
            if (!string.IsNullOrEmpty(filter))
            {
                FilteredFiles.Clear();

                foreach (var file in Handle.Data.Files)
                {
                    //add all files matching
                    if (checkContainLowerCase(file, filter))
                        FilteredFiles.Add(file);
                }
            }
            else
            {
                //reset filter if empty
                //make deep copy
                FilteredFiles = new ObservableCollection<Data.FileData>(Handle.Data.Files);
            }
        }

        #region event

        private void box_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterListBox(((TextBox)sender).Text);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint tag = (uint)((FrameworkElement)sender).Tag;

            ListItemPlay(tag, true);
        }

        private void menu_openContext_Click(object sender, RoutedEventArgs e)
        {
            //that's ugly, but it gets the 'grandParent' to open the context
            var listElement = sender as FrameworkElement;
            if (listElement != null)
            {
                //parent is the Grid containing this button
                var parent = listElement.Parent as FrameworkElement;
                if (parent != null)
                {
                    //grandParent is the outer Grid
                    var grandParent = parent.Parent as FrameworkElement;
                    if (grandParent != null)
                    {
                        context_ContextMenuOpening(grandParent, null);
                        grandParent.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        private void context_addToQueue_Clicked(object sender, RoutedEventArgs e)
        {
            //play the clicked file
            uint tag = (uint)((FrameworkElement)sender).Tag;
            ListItemPlay(tag, false);
        }

        private void context_createAndAddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //create menu, to create new playlsit
            var location = this.PointToScreen(new Point(0, 0));
            var dialog = new PlaylistAddDialog(location.X, location.Y, this.ActualWidth, this.ActualHeight);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                //create new playlist from dialog result
                Handle.Data.Playlists.Add(new Data.Playlist(dialog.PlaylistName));
            }
            else
                return;

            //id from last index, because it's last added from step above
            uint listId = Handle.Data.Playlists[(int)(Handle.Data.Playlists.Count - 1)].Id;

            addTitleToList(listId, list_All.SelectedItems);
        }

        private void context_AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //tag of sender is id of playlsit
            uint listId = (uint)((FrameworkElement)sender).Tag;

            addTitleToList(listId, list_All.SelectedItems);
        }

        private void addTitleToList(uint listId, IList selectedFiles)
        {
            //find list from listId
            Data.Playlist toAddList = null;
            foreach (var list in Handle.Data.Playlists)
            {
                if (list.Id == listId)
                    toAddList = list;
            }

            //all selected files
            foreach (var file in selectedFiles)
            {
                //search for title with tag, add this to lsit
                foreach (var title in Handle.Data.Files)
                {
                    //add
                    if (title.Id == ((Data.FileData)file).Id && toAddList != null)
                        toAddList.Tracks.Add(title);
                }
            }
        }

        private void context_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //create context menu
            uint tag = (uint)((FrameworkElement)sender).Tag;
            var context = ((FrameworkElement)sender).ContextMenu;

            context.Items.Clear();

            //add to queue menu
            var queueItem = new MenuItem();
            queueItem.Header = "Add to queue";
            queueItem.Tag = tag;
            queueItem.Click += context_addToQueue_Clicked;

            //add to context queue
            context.Items.Add(queueItem);

            //add to playlist menu
            var playlistItem = new MenuItem();
            playlistItem.Header = "Add to playlist...";
            playlistItem.Tag = tag;

            //new playlist item
            var newListItem = new MenuItem();
            newListItem.Header = "New Playlist...";
            newListItem.Tag = tag;
            newListItem.Click += context_createAndAddPlaylist_Click;

            //add new playlist to playlistItem
            playlistItem.Items.Add(newListItem);

            //any existing playlist item
            foreach (var list in Handle.Data.Playlists)
            {
                var playlist = new MenuItem();
                playlist.Header = list.Name;
                playlist.Tag = list.Id;
                playlist.Click += context_AddPlaylist_Click;
                //add each list to playlist items
                playlistItem.Items.Add(playlist);
            }
            //add playlist items to context menu
            context.Items.Add(playlistItem);
        }
    }

#pragma warning restore CS1591
}