using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DiscordBot.Data;
using MaterialDesignThemes.Wpf;
using VideoLibrary;
using YoutubeSearch;

namespace DiscordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for StreamMode.xaml
    /// </summary>
    public partial class StreamMode : UserControl, INotifyPropertyChanged
    {
        public delegate void PlayVideoHandler(Data.BotData data);

        public PlayVideoHandler PlayVideo;

        
        private ObservableCollection<Data.VideoData> suggestions = new ObservableCollection<VideoData>();
   

        public ObservableCollection<Data.VideoData> Suggestions
        {
            get => suggestions;
            set
            {
                suggestions = value;
                OnPropertyChanged("Suggestions");
            }
        }

        

        private string url = "";

        public string Url
        { get => url;
            set
            {
                url = value;
                ImageUri = IO.YTManager.getUrlToThumbnail(value);
                setTitleAsync(value);
                OnPropertyChanged("Url");
            }
        }

        private string title = "";

        public string Title
        { get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        private string imageUri = "";

        public string ImageUri
        { get => imageUri;
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

            list_History.DataContext = Handle.Data.VideoHistory;
 
        }


        private void startStream(Data.BotData data)
        {
            PlayVideo(data);
            Handle.Data.VideoHistory.addVideo(new Data.VideoData(Url, Title, ImageUri));
        }


        private async void setTitleAsync(string url)
        {
            try
            {
                Title =  await IO.YTManager.GetTitleTask(url);
            }
            catch
            {
                Title = "";
            }
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
                performSearch(Url);
            }
        }

        private void performSearch(string filter)
        {
            Suggestions.Clear();

            const int pages =1;
            var items = new VideoSearch();

            foreach (var item in items.SearchQuery(filter, pages))
            {
                Suggestions.Add(new Data.VideoData(item.Url, item.Title, item.Thumbnail));   
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
                //enqueue BotData item with stream
                //reference
                startStream(new BotData(Title)
                {
                    stream = stream,
                });
            }
            else
            {
               //fall back to caching to disk
                Handle.SnackbarWarning("Caching, this may take a while...");

                //alternatively try to download the video
                loadProgress.Visibility = Visibility.Visible;

                string location = await IO.YTManager.cacheVideo(vid);

                loadProgress.Visibility = Visibility.Collapsed;

                if (location != null)
                    startStream(new BotData(Title, location));
            }

            vid = null;
        }    

        #region events

        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            Url = ((TextBox) sender).Text;

            if (e.Key == Key.Enter)
            {               
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

        private void list_History_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                Url = fe.Tag.ToString();
                
            }
        }

        #endregion events

        #region property changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {          
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));           
        }

        #endregion property changed     
    }

#pragma warning restore CS1591
}