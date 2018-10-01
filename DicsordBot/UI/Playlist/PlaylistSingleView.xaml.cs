using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DicsordBot.UI.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistSingleView.xaml
    /// </summary>
    public partial class PlaylistSingleView : UserControl, INotifyPropertyChanged, IDropTarget
    {
        public delegate void SinglePlaylistItemEnqueuedHandler(Data.FileData file);

        public SinglePlaylistItemEnqueuedHandler SinglePlaylistItemEnqueued;

        public delegate void SinglePlaylistStartPlayHandler(uint listIndex, uint fileIndex);

        public SinglePlaylistStartPlayHandler SinglePlaylistStartPlay;

        public delegate void LeaveSingleViewHandler();

        public LeaveSingleViewHandler LeaveSingleView;

        private uint listIndex = 0;
        private bool isTopSelectionBarOpen = false;

        private ObservableCollection<Data.FileData> filteredFiles;
        private string Filter { get; set; }

        public Data.Playlist Playlist { get { return Handle.Data.Playlists[(int)listIndex]; } set { Handle.Data.Playlists[(int)listIndex] = value; } }
        public ObservableCollection<Data.FileData> PlaylistFiles { get { return Playlist.Tracks; } set { Playlist.Tracks = value; OnPropertyChanged("PlaylistFiles"); } }
        public ObservableCollection<Data.FileData> FilteredFiles { get { return filteredFiles; } set { filteredFiles = value; OnPropertyChanged("FilteredFiles"); } }

        public PlaylistSingleView(uint _listId)
        {
            //get index of playlist
            for (int i = 0; i < Handle.Data.Playlists.Count; i++)
            {
                if (Handle.Data.Playlists[i].Id == _listId)
                    listIndex = (uint)i;
            }
            //deep copy
            FilteredFiles = new ObservableCollection<Data.FileData>(PlaylistFiles);
            InitializeComponent();
            this.DataContext = this;
        }

        private void filterListBox(string filter)
        {
            //clear list and apply filter
            if (!string.IsNullOrEmpty(filter))
            {
                FilteredFiles.Clear();
                try
                {
                    foreach (var file in PlaylistFiles)
                    {
                        //add all files matching
                        if (IO.FileWatcher.checkForLowerMatch(file, filter))
                            FilteredFiles.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    UnhandledException.initWindow(ex, "whilest trying to rescan your files (debug)");
                }
            }
            else
            {
                //reset filter if empty
                //make deep copy
                FilteredFiles = new ObservableCollection<Data.FileData>(PlaylistFiles);
            }
        }

        private void ProcessDialogResult(bool result, bool isToDelete, string playlistName, string imagePath)
        {
            if (result == true)
            {
                Playlist.Name = playlistName;
                if (imagePath != null)
                    Playlist.ImagePath = imagePath;
            }
            else if (result == false && isToDelete == true)
            {
                Handle.Data.Playlists.RemoveAt((int)listIndex);
                LeaveSingleView();
            }
        }

        #region control events

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //only on double click
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                //start to replay the complete list
                uint fileId = (uint)((FrameworkElement)sender).Tag;

                //get the index of the tagged file
                for (int i = 0; i < PlaylistFiles.Count; i++)
                {
                    if (PlaylistFiles[i].Id == fileId)
                    {
                        SinglePlaylistStartPlay(listIndex, (uint)i);
                        break;
                    }
                }
            }
        }

        private void btn_playItem_Click(object sender, RoutedEventArgs e)
        {
            uint tag = (uint)((FrameworkElement)sender).Tag;

            SinglePlaylistStartPlay(listIndex, tag);
        }

        private void addMultipleToQueue_Click(object sender, RoutedEventArgs e)
        {
            foreach (Data.FileData selected in list_All.SelectedItems)
                SinglePlaylistItemEnqueued(selected);
        }

        private void box_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterListBox(((TextBox)sender).Text);
        }

        private void menu_openContext_Click(object sender, RoutedEventArgs e)
        {
            //find grandparent to open context
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

        private void list_All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //open or close the top selection bar, based on selected items
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

        private void stack_list_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                while (list_All.SelectedItems.Count > 0)
                {
                    var item = list_All.SelectedItems[0];

                    for (int i = 0; i < PlaylistFiles.Count; i++)
                    {
                        if ((Data.FileData)item == PlaylistFiles[i])
                        {
                            PlaylistFiles.RemoveAt(i);
                            filterListBox(box_Filter.Text);
                            break;
                        }
                    }
                }
            }
        }

        private void btn_editList_Click(object sender, RoutedEventArgs e)
        {
            openDialog(Playlist.Name, Application.Current.MainWindow, Playlist.ImagePath);
        }

        private void openDialog(string name, Window window, string imagePath)
        {
            //show edit window
            IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new PlaylistAddPopup(name, window, imagePath);
            popup.IsOpen = true;

            popup.Closed += delegate (object dSender, EventArgs dE)
            {
                IO.BlurEffectManager.ToggleBlurEffect(false);
                ProcessDialogResult(popup.Result, popup.IsToDelete, popup.PlaylistName, popup.ImagePath);
            };
        }

        #endregion control events

        #region notifyProperty

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion notifyProperty

        #region drag and drop

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            var sourceItem = dropInfo.Data as Data.FileData;

            if (sourceItem != null)
                dropInfo.Effects = DragDropEffects.Move;
            else
                dropInfo.Effects = DragDropEffects.Link;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            Data.FileData sourceItem = dropInfo.Data as Data.FileData;
            //move item from list to another position
            if (sourceItem != null)
            {
                Data.FileData targetItem = dropInfo.TargetItem as Data.FileData;
                //insert sourceItem to new place
                int oldIndex = PlaylistFiles.IndexOf(sourceItem);
                PlaylistFiles.RemoveAt(oldIndex);

                dropItem(dropInfo.InsertIndex, sourceItem);
            }
            //insert new item
            else
            {
                //convert item into Data.FileData

                IDataObject obj = dropInfo.Data as IDataObject;
                if (obj != null & obj.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])obj.GetData(DataFormats.FileDrop);
                    foreach (var track in files)
                    {
                        if (IO.FileWatcher.checkForValidFile(track))
                        {
                            dropItem(dropInfo.InsertIndex, IO.FileWatcher.getAllFileInfo(track));
                        }
                    }
                }
            }
        }

        private void dropItem(int index, Data.FileData file)
        {
            //drop on new position
            if (index < PlaylistFiles.Count)
                PlaylistFiles.Insert(index, file);
            else
                PlaylistFiles.Add(file);

            filterListBox(box_Filter.Text);
        }

        #endregion drag and drop
    }

#pragma warning restore CS1591
}