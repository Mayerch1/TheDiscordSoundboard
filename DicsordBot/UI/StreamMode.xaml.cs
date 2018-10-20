using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DiscordBot.Data;
using VideoLibrary;

namespace DiscordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for StreamMode.xaml
    /// </summary>
    public partial class StreamMode : UserControl, INotifyPropertyChanged
    {
        //this is Discord typo fix
        //this is DiscordBot typo fix

        public delegate void PlayVideoHandler(Data.BotData data);

        public PlayVideoHandler PlayVideo;

        private string url = "";

        public string Url
        {
            get => url;
            set
            {
                url = value;
                ImageUri = IO.YTManager.getUrlToThumbnail(value);
                OnPropertyChanged("Url");
            }
        }

        private string title = "";

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        private string imageUri = "";

        public string ImageUri
        {
            get => imageUri;
            set
            {
                imageUri = value;
                ImageChanged(value);
                OnPropertyChanged("ImageUri");
            }
        }

        public StreamMode()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        private void box_link_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
                Url = box.Text;
        }


        private void proccessEntry()
        {
            if (Url.Contains("http://") || Url.Contains("https://"))
            {
                getAndStartStream();
            }
            else
            {
                //start a search for searchterm saved in Url
            }
        }

        private async void getAndStartStream()
        {
            //download video
            Video vid = await IO.YTManager.getVideoAsync(Url);          
            if (vid == null) return;

            Title = vid.Title;

            //get playable stream
            Stream stream = await IO.YTManager.getStreamAsync(vid);
            //disable it for now 
            if (stream != null && !Handle.Data.Persistent.AlwaysCacheVideo)
            {              
                //enqueue BotData item with stream as reference
                PlayVideo(new BotData(Title)
                {
                    stream = stream,
                });
            }
            else
            {
                //fall back to caching to disk
                Handle.SnackbarWarning("Caching, this may take a while...");
                //alternatively try to download the video
                string location = await IO.YTManager.cacheVideo(vid);

                if (location != null)
                    PlayVideo(new BotData(Title, location));
            }

            //TODO: test if that deletes source for stream
            vid = null;
        }

        #region events

        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            Url = ((TextBox) sender).Text;

            if (e.Key == Key.Enter)
            {
                Url = ((TextBox) sender).Text;
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