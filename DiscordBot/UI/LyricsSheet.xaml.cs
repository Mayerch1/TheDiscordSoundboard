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

        public void setTitle(string title)
        {
            box_Title.Text = title;
        }

        public void setAuth(string auth)
        {
            box_Auth.Text = auth;
        }

        public void setLyric(string lyric)
        {
            txt_lyric.Text = lyric;
        }


        private void btn_RefetchLyrics_Click(object sender, RoutedEventArgs e)
        {
            var lyric = Util.IO.LyricsManager.getLyrics(box_Title.Text, box_Auth.Text);
            if (lyric != null)
            {
                setLyric(lyric.Lyric);
            }
        }


        private void btn_DirectMode_Toggle(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                if (btn.IsChecked == true)
                    Handle.Bot.IsDirectMode = true;
                else
                    Handle.Bot.IsDirectMode = false;
            }
        }
    }
#pragma warning restore CS1591
}