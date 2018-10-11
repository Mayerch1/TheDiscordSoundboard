using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using VideoLibrary;

namespace DicsordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for StreamMode.xaml
    /// </summary>
    public partial class StreamMode : UserControl, INotifyPropertyChanged
    {
        public delegate void PlayVideoHandler(Video vid);

        public PlayVideoHandler PlayVideo;

        private string lastCache = null;
        private string url = "";
        public string Url { get { return url; } set { url = value; OnPropertyChanged("Url"); } }

        private string title = "";
        public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }

        public StreamMode()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private async void requestStartStream()
        {
            //download video
            Video vid = await IO.YTManager.getVideoAsync(Url);

            if (vid != null)
            {
                //cache video, thread will then start replay
                Title = vid.Title;
                PlayVideo(vid);
                //cacheAndStreamVideo(vid);
            }
        }

        private async void cacheAndStreamVideo(Video vid)
        {
            Console.WriteLine("Caching: " + vid.Title + "...");

            //var x = await vid.StreamAsync();

            string location = await IO.YTManager.cacheVideo(vid);
            if (location != null)
                return;
            //sendVideo(location);
        }

        private void sendVideo(string path)
        {
            //send the  delegate to stream the file
            //if (path != null)
            //PlayVideo(new Data.ButtonData(Title, path));
        }

        #region events

        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Url = ((TextBox)sender).Text;
                requestStartStream();
            }
        }

        private void btn_Stream_Click(object sender, RoutedEventArgs e)
        {
            requestStartStream();
        }

        #endregion events

        #region property changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion property changed
    }

#pragma warning restore CS1591
}