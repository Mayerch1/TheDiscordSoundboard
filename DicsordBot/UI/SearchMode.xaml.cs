using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DiscordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// This class is a shame for anyone who wants to produce clean code
    /// </summary>
    public partial class SearchMode : UserControl, INotifyPropertyChanged
    {
        private bool isTopSelectionBarOpen = false;

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

        #region edit playlists

        private void addSingleTitleToList(uint listId, uint fileId)
        {
            //find list from listId
            Data.Playlist toAddList = null;
            foreach (var list in Handle.Data.Playlists)
            {
                if (list.Id == listId)
                    toAddList = list;
            }

            //search for fileTag in list
            foreach (var title in Handle.Data.Files)
            {
                if (title.Id == fileId)
                {
                    toAddList.Tracks.Add(title);
                    break;
                }
            }
        }

        private void addMultipleTitlesToList(uint listId, IList selectedFiles)
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

        #endregion edit playlists

        #region add_item_to events

        #region single add

        private void context_openInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is string path)
            {
                if (System.IO.File.Exists(path))
                {
                    string arg = "/select, \"" + path + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", arg);
                }
            }
        }

        private void context_createAndAddSingleToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //create menu, to create new playlsit

            IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new Playlist.PlaylistAddPopup(Application.Current.MainWindow);
            popup.IsOpen = true;

            popup.Closed += delegate (object dSender, EventArgs dE)
            {
                IO.BlurEffectManager.ToggleBlurEffect(false);
                //get tag
                uint fileTag = (uint)((FrameworkElement)((FrameworkElement)sender).Parent).Tag;

                //revert effectc, process (create, add) playlist
                ProcessSingleAddDialog(popup.Result, popup.PlaylistName, popup.ImagePath, fileTag);
            };
        }

        private void ProcessSingleAddDialog(bool result, string playlistName, string imagePath, uint fileTag)
        {
            if (result == true)
                //create new playlist from dialog result
                Handle.Data.Playlists.Add(new Data.Playlist(playlistName, imagePath));
            else
                return;

            //id from last index, because it's last added from step above
            uint listId = Handle.Data.Playlists[(int)(Handle.Data.Playlists.Count - 1)].Id;

            addSingleTitleToList(listId, fileTag);
        }

        private void context_AddSingleToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //tag of sender is id of playlsit
            uint listId = (uint)((FrameworkElement)sender).Tag;
            //get tag of parent
            uint fileTag = (uint)((FrameworkElement)((FrameworkElement)sender).Parent).Tag;

            addSingleTitleToList(listId, fileTag);
        }

        private void context_addSingleToQueue_Clicked(object sender, RoutedEventArgs e)
        {
            //play the clicked file
            uint tag = (uint)((FrameworkElement)sender).Tag;
            ListItemPlay(tag, false);
        }

        #endregion single add

        #region multiple add

        private void context_createAndAddMultipleToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //create menu, to create new playlsit

            IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new Playlist.PlaylistAddPopup(Application.Current.MainWindow);
            popup.IsOpen = true;

            popup.Closed += delegate (object dSender, EventArgs dE)
            {
                IO.BlurEffectManager.ToggleBlurEffect(false);
                ProcessMultipleAddDialog(popup.Result, popup.PlaylistName, popup.ImagePath);
            };
        }

        private void ProcessMultipleAddDialog(bool result, string playlistName, string imagePath)
        {
            if (result == true)
                //create new playlist from dialog result
                Handle.Data.Playlists.Add(new Data.Playlist(playlistName, imagePath));
            else
                return;

            //new playlist is on last index
            uint listId = Handle.Data.Playlists[(int)(Handle.Data.Playlists.Count - 1)].Id;

            addMultipleTitlesToList(listId, list_All.SelectedItems);
        }

        private void context_AddMultipleToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //tag of sender is id of playlsit
            uint listId = (uint)((FrameworkElement)sender).Tag;
            //get tag of parent

            addMultipleTitlesToList(listId, list_All.SelectedItems);
        }

        private void btn_addMultipleToQueue_Clicked(object sender, RoutedEventArgs e)
        {
            foreach (Data.FileData selected in list_All.SelectedItems)
            {
                ListItemPlay(selected.Id, false);
            }
        }

        #endregion multiple add

        #endregion add_item_to events

        #region manage context

        private MenuItem getQueueItem(RoutedEventHandler clicked, uint fileTag)
        {
            //'add to queue' menu
            var queueItem = new MenuItem()
            {
                Header = "Add to queue",
                Tag = fileTag,
            };
            queueItem.Click += clicked;

            return queueItem;
        }

        private MenuItem getOpenExplorerItem(uint fileTag)
        {
            //get file of tag
            string path = "";
            foreach (var file in Handle.Data.Files)
            {
                if (file.Id == fileTag)
                {
                    path = file.Path;
                    break;
                }
            }

            var openExplorer = new MenuItem()
            {
                Header = "Open in Explorer",
                //this will be opened on click
                Tag = path,
            };
            openExplorer.Click += context_openInExplorer_Click;

            return openExplorer;
        }

        private List<MenuItem> getPlaylistItems(RoutedEventHandler newPlaylistHandler, RoutedEventHandler existingPlaylistHandler, uint fileTag = 0)
        {
            //'add to playlist...' menu
            List<MenuItem> playListItems = new List<MenuItem>();

            //new playlist item as submenu
            var newListItem = new MenuItem()
            {
                Header = "New Playlist...",
                Tag = fileTag,
            };
            newListItem.Click += newPlaylistHandler;
            playListItems.Add(newListItem);

            //any existing playlist item as submenu
            foreach (var list in Handle.Data.Playlists)
            {
                var playlist = new MenuItem()
                {
                    Header = list.Name,
                    Tag = list.Id,
                };
                playlist.Click += existingPlaylistHandler;
                //add each list to playlist items
                playListItems.Add(playlist);
            }

            return playListItems;
        }

        private void populate_AddSingle_Context(ContextMenu context, uint fileTag)
        {
            //context for adding one single item
            context.Items.Clear();

            //add to context queue
            context.Items.Add(getQueueItem(context_addSingleToQueue_Clicked, fileTag));

            //add to playlist outer-menu
            var playlistItem = new MenuItem()
            {
                Header = "Add to playlist...",
                Tag = fileTag,
            };

            foreach (var menu in getPlaylistItems(context_createAndAddSingleToPlaylist_Click, context_AddSingleToPlaylist_Click, fileTag))
            {
                playlistItem.Items.Add(menu);
            }
            //add playlist items to context menu
            context.Items.Add(playlistItem);

            //add "open in Explorer" menu
            context.Items.Add(getOpenExplorerItem(fileTag));
        }

        private void populate_AddMultiple_Context(ContextMenu context)
        {
            //context for adding multiple items
            context.Items.Clear();

            foreach (var menu in getPlaylistItems(context_createAndAddMultipleToPlaylist_Click, context_AddMultipleToPlaylist_Click))
            {
                context.Items.Add(menu);
            }
        }

        #endregion manage context

        #region events

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //only on double click
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                uint tag = (uint)((FrameworkElement)sender).Tag;

                ListItemPlay(tag, true);
            }
        }

        private void btn_playItem_Click(object sender, RoutedEventArgs e)
        {
            uint tag = (uint)((FrameworkElement)sender).Tag;

            ListItemPlay(tag, true);
        }

        private void context_addMultiple_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //create context menu, for multiple add

            var context = ((FrameworkElement)sender).ContextMenu;
            populate_AddMultiple_Context(context);
        }

        private void btn_addMultiple_Click(object sender, RoutedEventArgs e)
        {
            //add context menu to button
            var context = ((FrameworkElement)sender).ContextMenu;

            populate_AddMultiple_Context(context);

            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }

        private void context_addSingle_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //create context menu, for single add
            uint tag = (uint)((FrameworkElement)sender).Tag;
            var context = ((FrameworkElement)sender).ContextMenu;

            populate_AddSingle_Context(context, tag);
        }

        private void btn_addSingle_Click(object sender, RoutedEventArgs e)
        {
            //that's ugly, but it gets the 'grandParent' to open the context
            //#prettyCodeAward2018
            if (sender is FrameworkElement listElement)
            {
                //parent is the Grid containing this button
                if (listElement.Parent is FrameworkElement parent)
                {
                    //grandParent is the outer Grid
                    if (parent.Parent is FrameworkElement grandParent)
                    {
                        var context = grandParent.ContextMenu;
                        uint fileTag = (uint)grandParent.Tag;

                        //select the currently pressed listBoxItem
                        for (int i = 0; i < list_All.Items.Count; i++)
                        {
                            if (((Data.FileData)list_All.Items[i]).Id == fileTag)
                            {
                                list_All.SelectedIndex = i;
                                break;
                            }
                        }

                        populate_AddSingle_Context(context, fileTag);
                        grandParent.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Left button down");
        }

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Right Button Down");
        }

        private void list_All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListBox)sender;
            if (list.SelectedItems.Count >= 2 && !isTopSelectionBarOpen)
            {
                Storyboard sb;
                sb = FindResource("OpenTopSelectionBar") as Storyboard;
                sb.Begin();
                isTopSelectionBarOpen = true;
            }
            else if (list.SelectedItems.Count <= 1 && isTopSelectionBarOpen)
            {
                Storyboard sb;
                sb = FindResource("CloseTopSelectionBar") as Storyboard;
                sb.Begin();
                isTopSelectionBarOpen = false;
            }
        }

        private void list_All_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (list_All.SelectedItem is Data.FileData item)
                    ListItemPlay(item.Id, true);
            }
        }

        private void box_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = ((TextBox)sender).Text;

            //refresh after list is filtered, minimizes render Time
            filteredFiles = IO.FileWatcher.filterList(filter, Handle.Data.Files);
            OnPropertyChanged("FilteredFiles");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion events
    }

#pragma warning restore CS1591
}