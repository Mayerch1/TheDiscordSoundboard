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
            var filter = ((TextBox)sender).Text;
            FilteredFiles = IO.FileWatcher.filterList(filter, PlaylistFiles);
        }

        private void menu_openContext_Click(object sender, RoutedEventArgs e)
        {
            //find grandparent to open context
            if (sender is FrameworkElement listElement)
            {
                if (listElement.Parent is FrameworkElement parent)
                {
                    if (parent.Parent is FrameworkElement grandParent)
                    {
                        grandParent.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        private void menu_openInExplorer_Clicked(object sender, RoutedEventArgs e)
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
            if (e.Key == Key.Enter)
            {
                //start replay of file
                if (list_All.SelectedItem is Data.FileData item)
                {
                    SinglePlaylistStartPlay(listIndex, item.Id);
                }
            }
            else if (e.Key == Key.Delete)
            {
                //remove file[s] from playlist
                while (list_All.SelectedItems.Count > 0)
                {
                    var item = list_All.SelectedItems[0];
                    //find corresponding item
                    for (int i = 0; i < PlaylistFiles.Count; i++)
                    {
                        if ((Data.FileData)item == PlaylistFiles[i])
                        {
                            PlaylistFiles.RemoveAt(i);
                            FilteredFiles = IO.FileWatcher.filterList(box_Filter.Text, PlaylistFiles);
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

            var popup = new PlaylistAddPopup(window, name, imagePath);
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

            if (dropInfo.Data is Data.FileData sourceItem)

                dropInfo.Effects = DragDropEffects.Move;
            else
                dropInfo.Effects = DragDropEffects.Link;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            //move item from list to another position
            if (dropInfo.Data is Data.FileData sourceItem)
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

            FilteredFiles = IO.FileWatcher.filterList(box_Filter.Text, PlaylistFiles);
        }

        #endregion drag and drop
    }

#pragma warning restore CS1591
}