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

namespace DicsordBot
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistAddDialog.xaml
    /// </summary>
    public partial class PlaylistAddDialog : Window
    {
        public string PlaylistName { get { return box_Name.Text; } set { box_Name.Text = value; } }

        public PlaylistAddDialog(double x, double y, double pWidth, double pHeight)
        {
            InitializeComponent();
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
    }

#pragma warning restore CS1591
}