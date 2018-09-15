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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for PlaylistMode.xaml
    /// </summary>
    public partial class PlaylistMode : UserControl
    {
        public delegate void PlaylistItemPlayHandler(uint tag, uint fileIndex);

        public PlaylistItemPlayHandler PlaylistItemPlay;

        public PlaylistMode()
        {
            InitializeComponent();
        }

        private void btn_playlistAdd_Click(object sender, RoutedEventArgs e)
        {
            PlaylistItemPlay(0, 0);
            //TODO: playlist add dialog or dropdown
        }
    }

#pragma warning restore CS1591
}