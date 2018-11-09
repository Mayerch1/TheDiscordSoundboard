using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PlaylistModule.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistAddPopup.xaml
    /// </summary>
    public partial class PlaylistAddPopup : Popup, INotifyPropertyChanged
    {
        private string imagePath = DataManagement.Playlist.defaultImage;

        public string PlaylistName { get { return box_Name.Text; } set { box_Name.Text = value; } }

        public string ImagePath { get { return imagePath; } set { imagePath = value; ImageChanged(value); } }

        public bool IsToDelete { get; set; } = false;
        public bool Result { get; set; } = false;

        public PlaylistAddPopup(Window window, string currentName = "", string _imagePath = DataManagement.Playlist.defaultImage)
        {
            this.StaysOpen = false;
            InitializeComponent();

            PlaylistName = currentName;
            ImagePath = _imagePath;

            initPosition(window);

            box_Name.SelectAll();
            

            this.DataContext = this;
        }

        private void initPosition(Window window)
        {
            this.PlacementTarget = window;
            this.Placement = PlacementMode.Center;
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            Result = true;

            IsToDelete = false;
            this.IsOpen = false;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            this.StaysOpen = true;
            if (MessageBox.Show("Are you sure to delete this Playlist?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Result = false;
                IsToDelete = true;

                this.IsOpen = false;
            }
            this.StaysOpen = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = false;
                IsToDelete = false;
                this.IsOpen = false;
            }
        }

        private void btn_ListImage_Click(object sender, RoutedEventArgs e)
        {
            this.StaysOpen = true;

            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();

            if (System.IO.Directory.Exists(ImagePath))
                fileDialog.InitialDirectory = ImagePath;
            else
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && fileDialog.CheckFileExists)
            {
                if (fileDialog.FileName != DataManagement.Playlist.defaultImage)
                {
                    //cache image
                    ImagePath = Util.IO.ImageManager.cacheImage(fileDialog.FileName);
                }
            }

            //TODO: double click on image will most likely close the popup

            this.StaysOpen = false;
        }

        #region NonTopmostPopup

        protected override void OnOpened(EventArgs e)
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            RECT rect;

            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion P/Invoke imports & definitions

        #endregion NonTopmostPopup

        #region events

        private void ImageChanged(string path)
        {
            //default path is already set
            if (path != DataManagement.Playlist.defaultImage && System.IO.File.Exists(path))
            {
                btn_Image.Background = new System.Windows.Media.ImageBrush(new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)));
            }
        }

        /// <summary>
        /// PropertyChanged Event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// propertychanged method, notifies the actual handler
        /// </summary>
        /// <param name="info"></param>
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        #endregion events
    }

#pragma warning disable CS1591
}