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
using DataManagement;
using Util.IO;

namespace StreamModule
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for StreamMode.xaml
    /// </summary>
    public partial class StreamMode : UserControl, INotifyPropertyChanged
    {
        public delegate void PlayVideoHandler(BotData data);

        public PlayVideoHandler PlayVideo;

        public delegate void QueueVideoHandler(BotData data);

        public QueueVideoHandler QueueVideo;

        public delegate void EulaRejectHandler();

        public EulaRejectHandler EulaRejected;


        private ObservableCollection<VideoData> _suggestions = new ObservableCollection<VideoData>();
        private string _url = "";
        private string _title = "Video Title";
        private string _imageUri = "";
        private string _duration = "0:00";
        private RuntimeData _data;



        public RuntimeData Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public ObservableCollection<VideoData> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = value;
                OnPropertyChanged("Suggestions");
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                SetMetaDataAsync(value);
                OnPropertyChanged("Url");
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string ImageUri
        {
            get => _imageUri;
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


        public StreamMode(DataManagement.RuntimeData dt)
        {
            Data = dt;
            //-------------------
            InitializeComponent();
            list_History.DataContext = Data.VideoHistory;

            //--------------------
            //show legal warning
            //-------------------
            if (!Data.Persistent.IsEulaAccepted)
            {
                
                Util.IO.BlurEffectManager.ToggleBlurEffect(true);


                var popup = new StreamWarningPopup(Application.Current.MainWindow);

                popup.Closed += delegate (object pSender, EventArgs pArgs)
                {

                    Util.IO.BlurEffectManager.ToggleBlurEffect(false);

                    Data.Persistent.IsEulaAccepted = popup.eula;

                    //return, if eula was rejected
                    if (!popup.eula)
                        EulaRejected();
                };

                popup.IsOpen = true;
            }
        }


        private void StartStream(BotData data)
        {
            PlayVideo(data);
            Data.VideoHistory.addVideo(new VideoData(Url, Title, ImageUri, Duration), Data.Persistent.MaxVideoHistoryLen);
        }

        private void QueueStream(BotData data)
        {
            QueueVideo(data);
            Data.VideoHistory.addVideo(new VideoData(Url, Title, ImageUri, Duration), Data.Persistent.MaxVideoHistoryLen);
        }


        private async void SetMetaDataAsync(string url)
        {
            string id = YTManager.getIdFromUrl(url);

            if (String.IsNullOrWhiteSpace(id))
                return;

            List<VideoInformation> infos;
            try
            {
                infos = await new VideoSearch().SearchQueryTaskAsync(id, 1);
            }
            catch
            {
                //fallback, if rate limit blocks api call
                ImageUri = YTManager.getUrlToThumbnail(url);
                Title = await YTManager.GetTitleTask(url);
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
                GetAndStartStream(Url);
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

            const int pages = 1;

            List<VideoInformation> result;

            try
            {
                result = await new VideoSearch().SearchQueryTaskAsync(filter, pages);
            }
            catch (Exception ex)
            {
                Util.IO.LogManager.LogException(ex, "StreamModule/StreamMode", "Could not accomplish search for video");
                return;
            }

            foreach (var item in result)
            {
                Suggestions.Add(new VideoData(item));
            }
        }


        private async void GetAndStartStream(string url, bool IsQueue = false)
        {
            if (url == null)
                return;

            loadProgress.Visibility = Visibility.Visible;
            //download video
            Video vid = await YTManager.getVideoAsync(url);
            if (vid == null)
            {
                
                SnackbarManager.SnackbarMessage("Cannot cache video.");
                loadProgress.Visibility = Visibility.Collapsed;
                return;
            }

            Title = vid.Title;

            //get playable stream
            Stream stream = await YTManager.getStreamAsync(vid);

            //disable it for now 
            if (stream != null && !Data.Persistent.AlwaysCacheVideo)
            {
                loadProgress.Visibility = Visibility.Collapsed;

                //enqueue BotData item with stream
                //reference

                if (IsQueue)
                {
                    QueueVideo(new BotData(Title)
                    {
                        stream = stream,
                    });
                }
                else
                {
                    StartStream(new BotData(Title)
                    {
                        stream = stream,
                    });
                }
            }
            else
            {
                //fall back to caching to disk
                  
                //alternatively try to download the video


                string location = await YTManager.cacheVideo(vid);

                loadProgress.Visibility = Visibility.Collapsed;

                if (location != null)
                {
                    if (IsQueue)
                        QueueVideo(new BotData(Title, location));
                    else
                        StartStream(new BotData(Title, location));
                }

            }
            vid = null;
        }

        #region events
        private void Recommended_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //intercept scroll event and make scroll viewer accept the wheel
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 4);
            e.Handled = true;
        }
        private void box_url_KeyDown(object sender, KeyEventArgs e)
        {
            Url = ((TextBox)sender).Text;

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

        private void context_addQueue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                GetAndStartStream(fe.Tag as string, true);
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