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

namespace DiscordBot.UI
{
#pragma warning disable CS1591
    /// <summary>
    /// Interaction logic for LyricsSheet.xaml
    /// </summary>
    public partial class LyricsSheet : UserControl
    {
        public LyricsSheet()
        {
            InitializeComponent();
        }

        public void setLyric(string lyric)
        {
            txt_lyric.Text = lyric;
        }

        public void setAuth(string auth)
        {
            txt_Auth.Text = auth;

        }

        public void setTitle(string title)
        {
            txt_Title.Text = title;
        }
    }
#pragma warning restore CS1591
}
