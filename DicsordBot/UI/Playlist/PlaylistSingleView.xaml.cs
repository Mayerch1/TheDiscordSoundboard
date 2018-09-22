﻿using System;
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

namespace DicsordBot.UI.Playlist
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

        public delegate void LeaveSingleViewHandler();

        public LeaveSingleViewHandler LeaveSingleView;

        public PlaylistSingleView(uint _index)
        {
            index = _index;
            InitializeComponent();
            this.DataContext = this;
            FilteredFiles = new ObservableCollection<Data.FileData>(PlaylistFiles);
        }

        private uint index = 0;
        private ObservableCollection<Data.FileData> filteredFiles;
        private string Filter { get; set; }
        public Data.Playlist Playlist { get { return Handle.Data.Playlists[(int)index]; } set { Handle.Data.Playlists[(int)index] = value; } }
        public ObservableCollection<Data.FileData> PlaylistFiles { get { return Playlist.Tracks; } set { Playlist.Tracks = value; OnPropertyChanged("PlaylistFiles"); } }
        public ObservableCollection<Data.FileData> FilteredFiles { get { return filteredFiles; } set { filteredFiles = value; OnPropertyChanged("FilteredFiles"); } }

        private void stack_list_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //only on double click
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                //start to replay the complete list
                uint tag = (uint)((FrameworkElement)sender).Tag;

                //get the index of the tagged file
                for (int i = 0; i < PlaylistFiles.Count; i++)
                {
                    if (PlaylistFiles[i].Id == tag)
                    {
                        SinglePlaylistStartPlay(index, (uint)i);
                        break;
                    }
                }
            }
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

        private void context_AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //This should crash
            //uint tag = (uint)((FrameworkElement)sender).Tag;

            //ListItemPlay(tag, false);
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var track in files)
                {
                    if (FileWatcher.checkForValidFile(track))
                    {
                        Data.FileData fileData = FileWatcher.getAllFileInfo(track);
                        Handle.Data.Playlists[(int)index].Tracks.Add(fileData);
                    }
                }
            }
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
                            filterListBox(Filter);
                            break;
                        }
                    }
                }
            }
        }

        private void btn_editList_Click(object sender, RoutedEventArgs e)
        {
            Point location = this.PointToScreen(new Point(0, 0));
            var dialog = new PlaylistAddDialog(Playlist.Name, location.X, location.Y, this.ActualWidth, this.ActualHeight);

            var result = dialog.ShowDialog();

            if (result == true)
                Playlist.Name = dialog.PlaylistName;
            else if (result == false && dialog.IsToDelete == true)
            {
                Handle.Data.Playlists.RemoveAt((int)index);
                LeaveSingleView();
            }
        }

        private void box_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterListBox(((TextBox)sender).Text);
            Filter = ((TextBox)sender).Text;
        }

        private void filterListBox(string filter)
        {
            //clear list and apply filter
            if (!string.IsNullOrEmpty(filter))
            {
                FilteredFiles.Clear();

                foreach (var file in PlaylistFiles)
                {
                    //add all files matching
                    if (FileWatcher.checkForLowerMatch(file, filter))
                        FilteredFiles.Add(file);
                }
            }
            else
            {
                //reset filter if empty
                //make deep copy
                FilteredFiles = new ObservableCollection<Data.FileData>(PlaylistFiles);
            }
        }
    }

#pragma warning restore CS1591
}