using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot.UI.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistAddPopup.xaml
    /// </summary>
    public partial class PlaylistAddPopup : Popup, INotifyPropertyChanged
    {
        private string imagePath = "/res/list-256.png";

        public string PlaylistName { get { return box_Name.Text; } set { box_Name.Text = value; } }

        public string ImagePath { get { return imagePath; } set { imagePath = value; OnPropertyChanged("ImagePath"); ImageChanged(value); } }

        public bool IsToReopen { get; set; } = false;

        private bool IsFileDialogOpened { get; set; } = false;

        public bool IsToDelete { get; set; } = false;
        public bool Result { get; set; } = false;
        private bool IsDialogOpen { get; set; } = false;

        public PlaylistAddPopup(Window window)
        {
            this.StaysOpen = false;
            InitializeComponent();
            initPosition(window);
            box_Name.SelectAll();
            box_Name.Focus();

            this.DataContext = this;
        }

        public PlaylistAddPopup(string currentName, Window window, string imagePath)
        {
            this.StaysOpen = false;
            InitializeComponent();

            PlaylistName = currentName;
            ImagePath = imagePath;

            initPosition(window);
            box_Name.SelectAll();
            box_Name.Focus();

            this.DataContext = this;
        }

        private void initPosition(Window window)
        {
            Point p = window.GetAbsolutePosition();

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
            IsDialogOpen = true;
            this.StaysOpen = true;
            if (MessageBox.Show("Are you shure to delete this Playlist?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsDialogOpen = false;
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
                ImagePath = fileDialog.FileName;
            }

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
            //imageBrush.ImageSource = new BitmapImage(new Uri(path));
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