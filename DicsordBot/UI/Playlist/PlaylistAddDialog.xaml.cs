using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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

        public PlaylistAddDialog(double x, double y, double pWidth, double pHeight)
        {
            InitializeComponent();
            initPosition(x, y, pWidth, pHeight);
            box_Name.SelectAll();
            box_Name.Focus();
        }

        public PlaylistAddDialog(string currentName, double x, double y, double pWidth, double pHeight)
        {
            InitializeComponent();
            PlaylistName = currentName;
            initPosition(x, y, pWidth, pHeight);
            box_Name.SelectAll();
            box_Name.Focus();
        }

        private void initPosition(double x, double y, double pWidth, double pHeight)
        {
            this.Left = x;
            this.Top = y;
            this.Width = pWidth;
            this.Height = pHeight;
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you shure to delete this Playlist?", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DialogResult = false;
                IsToDelete = true;
                this.Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                IsToDelete = false;
                this.Close();
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!brd_Dialog.IsMouseOver)
            {
                DialogResult = false;
                IsToDelete = false;
                this.Close();
            }
        }
    }

#pragma warning restore CS1591
}