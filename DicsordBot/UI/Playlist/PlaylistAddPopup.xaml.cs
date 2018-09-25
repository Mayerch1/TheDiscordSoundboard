using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for PlaylistAddPopup.xaml
    /// </summary>
    public partial class PlaylistAddPopup : Popup
    {
        public string PlaylistName { get { return box_Name.Text; } set { box_Name.Text = value; } }
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
        }

        public PlaylistAddPopup(string currentName, Window window)
        {
            this.StaysOpen = false;
            InitializeComponent();
            PlaylistName = currentName;
            initPosition(window);
            box_Name.SelectAll();
            box_Name.Focus();
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
            if (MessageBox.Show("Are you shure to delete this Playlist?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsDialogOpen = false;
                Result = false;
                IsToDelete = true;
                this.IsOpen = false;
            }
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
    }

#pragma warning disable CS1591
}