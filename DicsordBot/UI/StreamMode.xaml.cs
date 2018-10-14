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
        public delegate void PlayVideoHandler(Data.BotData data);

        public PlayVideoHandler PlayVideo;

        private string url = "";
        public string Url { get { return url; } set { url = value; ImageUri = IO.YTManager.getUrlToThumbnail(value); OnPropertyChanged("Url"); } }

        private string title = "";
        public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }

        private string imageUri = "";
        public string ImageUri { get { return imageUri; } set { imageUri = value; ImageChanged(value); OnPropertyChanged("ImageUri"); } }

        public StreamMode()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void proccessEntry()
        {
            if (Url.Contains("http://") || Url.Contains("https://"))
            {
                getAndStartStream();
            }
            else
            {
            }
        }

        private async void getAndStartStream()
        {
            //download video
            Video vid = await IO.YTManager.getVideoAsync(Url);

            if (vid != null)
            {
                //cache video, thread will then start replay
                Title = vid.Title;

                //PlayVideo(vid);
                cacheAndStreamVideo(vid);

                vid = null;
            }
        }

        private async void cacheAndStreamVideo(Video vid)
        {
            Console.WriteLine("Caching: " + vid.Title + "...");

            //var x = await vid.StreamAsync();

            string location = await IO.YTManager.cacheVideo(vid);

            if (location != null)
                sendVideo(location);
        }

        private void sendVideo(string path)
        {
            //send the  delegate to stream the file
            if (path != null)
            {
                PlayVideo(new Data.BotData(Title, path));
            }
        }

        #region events

        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            Url = ((TextBox)sender).Text;

            if (e.Key == Key.Enter)
            {
                Url = ((TextBox)sender).Text;
                proccessEntry();
            }
        }

        private void ImageChanged(string uri)
        {
            img_thumbnail.Source = new BitmapImage(new Uri(ImageUri, UriKind.RelativeOrAbsolute));
        }

        private void btn_Stream_Click(object sender, RoutedEventArgs e)
        {
            proccessEntry();
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