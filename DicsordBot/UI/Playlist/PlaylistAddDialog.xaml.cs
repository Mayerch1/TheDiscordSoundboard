using System;
using System.Windows;
using System.Windows.Input;

namespace DicsordBot.UI.Playlist
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistAddDialog.xaml
    /// </summary>
    public partial class PlaylistAddDialog : Window
    {
        public string PlaylistName { get { return box_Name.Text; } set { box_Name.Text = value; } }
        public bool IsToDelete { get; set; } = false;
        public bool Result { get; set; } = false;

        public PlaylistAddDialog(Window window)
        {
            InitializeComponent();
            initPosition(window);
            box_Name.SelectAll();
            box_Name.Focus();
        }

        public PlaylistAddDialog(string currentName, Window window)
        {
            InitializeComponent();
            PlaylistName = currentName;
            initPosition(window);
            box_Name.SelectAll();
            box_Name.Focus();
        }

        private void initPosition(Window window)
        {
            Point p = window.GetAbsolutePosition();

            this.Left = (p.X + window.ActualWidth / 2) - (this.Width / 2);
            this.Top = (p.Y + window.ActualHeight / 2) - (this.Height / 2);
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            IsToDelete = false;
            this.Close();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you shure to delete this Playlist?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Result = false;
                IsToDelete = true;
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = false;
                IsToDelete = false;
                this.Close();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Result = false;
            IsToDelete = false;
            try
            {
                this.Close();
            }
            catch
            {
                Console.WriteLine("Windows is already closing");
            }
        }
    }

#pragma warning restore CS1591
}