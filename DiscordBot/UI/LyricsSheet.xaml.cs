using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class LyricsSheet : UserControl, INotifyPropertyChanged
    {
        private const string lyricErr = "Could not find any lyrics for the given input\nMake sure to enter the exact title and the exact name of the Author.\nHit the search button to retry";
        private string lyrics;
        private string title;
        private string author;

        public string Lyrics
        {
            get => lyrics;
            set
            {
                lyrics = value;
                OnPropertyChanged("Lyrics");
            }
        }


        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Author
        {
            get => author;
            set
            {
                author = value;
                OnPropertyChanged("Author");
            }
        }

        

        //TODO change name from interpret into author/songwriter
        public LyricsSheet()
        {          
            InitializeComponent();
            this.DataContext = this;
            Lyrics = lyricErr;
        }



        private void btn_RefetchLyrics_Click(object sender, RoutedEventArgs e)
        {
            var result = Util.IO.LyricsManager.getLyrics(Title, Author);
            if (result != null)
            {
                //TODO see if writing back Author/Title is wanted
                Lyrics = result.Lyric;
                Title = result.LyricSong;
                Author = result.LyricArtist;
            }
            else
            {
                Lyrics = lyricErr;
            }
           
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
#pragma warning restore CS1591
}