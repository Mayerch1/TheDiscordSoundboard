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
            double finalX = (x + pWidth / 2) - (this.Width / 2);
            double finalY = (y + pHeight / 2) - (this.Height / 2);
            this.Left = finalX;
            this.Top = finalY;
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
    }

#pragma warning restore CS1591
}