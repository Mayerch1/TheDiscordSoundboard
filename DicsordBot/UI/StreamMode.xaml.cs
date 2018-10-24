using DiscordBot.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        
        private ObservableCollection<Data.VideoData> _suggestions = new ObservableCollection<VideoData>();
        private string _url = "";
        private string _title = "";
        private string _imageUri = "";
        private string _duration = "0:00";

        public ObservableCollection<Data.VideoData> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = value;
                OnPropertyChanged("Suggestions");
            }
        }
     
        public string Url
        { get => _url;
            set
            {
                _url = value;                              
                SetMetaDataAsync(value);
                OnPropertyChanged("Url");
            }
        }
     
        public string Title
        { get => _title;
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
     
        public string ImageUri
        { get => _imageUri;
            set
            {
                _imageUri = value;
                ImageChanged(value);
                OnPropertyChanged("ImageUri");
            }
        }

        public string Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }


        public StreamMode()
        {
            InitializeComponent();

            list_History.DataContext = Handle.Data.VideoHistory;
 
        }


        private void StartStream(Data.BotData data)
        {
            PlayVideo(data);
            Handle.Data.VideoHistory.addVideo(new Data.VideoData(Url, Title, ImageUri));
        }


        private async void SetMetaDataAsync(string url)
        {
            string id = IO.YTManager.getIdFromUrl(url);

            if (String.IsNullOrWhiteSpace(id))
                return;


            List<VideoInformation> infos;
            try
            {
                infos = await IO.YTManager.SearchQueryTaskAsync(id, 1);
            }
            catch(Exception ex)
            {
                //fallback, if rate limit blocks api call
                ImageUri = IO.YTManager.getUrlToThumbnail(url);
                Title = await IO.YTManager.GetTitleTask(url);
                return;
            }

            if (infos.Count > 0)
            {
                var videoInfo = infos[0];

                ImageUri = videoInfo.Thumbnail;
                Title = videoInfo.Title;
                Duration = videoInfo.Duration;
            }          
        }    

        private void box_link_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
                Url = box.Text;
        }

        
        private void ProcessEntry()
        {
            if (Url.Contains("http://") || Url.Contains("https://"))
            {
                GetAndStartStream();
            }
            else
            {
                //start a search for search term saved in Url
                PerformSearch(Url);
            }
        }

        private async void PerformSearch(string filter)
        {
            Suggestions.Clear();

            const int pages =1;

            List<VideoInformation> result;

            try
            {
                result = await IO.YTManager.SearchQueryTaskAsync(filter, pages);
            }
            catch
            {
                return;
            }
    
            foreach (var item in result)
            {  
                Suggestions.Add(new Data.VideoData(item));   
            }
        }

       


        private async void GetAndStartStream()
        {
            loadProgress.Visibility = Visibility.Visible;

            //download video
            Video vid = await IO.YTManager.getVideoAsync(Url);          
            if (vid == null) return;

            Title = vid.Title;

            //get playable stream
            Stream stream = await IO.YTManager.getStreamAsync(vid);

            //disable it for now 
            if (stream != null && !Handle.Data.Persistent.AlwaysCacheVideo)
            {
                loadProgress.Visibility = Visibility.Collapsed;

                //enqueue BotData item with stream
                //reference
                StartStream(new BotData(Title)
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

                loadProgress.Visibility = Visibility.Collapsed;

                if (location != null)
                    StartStream(new BotData(Title, location));
            }

            vid = null;
        }    

        #region events

        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            Url = ((TextBox) sender).Text;

            if (e.Key == Key.Enter)
            {               
                ProcessEntry();
            }
        }

        private void ImageChanged(string uri)
        {
            img_thumbnail.Source = new BitmapImage(new Uri(ImageUri, UriKind.RelativeOrAbsolute));
        }

        private void btn_Stream_Click(object sender, RoutedEventArgs e)
        {
            ProcessEntry();
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