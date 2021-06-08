using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VideoLibrary;
using DataManagement;
using NYoutubeDL.Models;
using Util.IO;
using System.Linq;

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
        private int progress = 50;
        private string _url = "";
        private string _title = "Video Title";
        private string _imageUri = "";
        private string _duration = "0:00";
        private string _author = "";
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

        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
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

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged("Author");
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

                popup.Closed += delegate(object pSender, EventArgs pArgs)
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
            Data.VideoHistory.addVideo(new VideoData(Url, Title, Author,ImageUri, Duration),
                Data.Persistent.MaxVideoHistoryLen);
        }

        private void QueueStream(BotData data)
        {
            QueueVideo(data);
            Data.VideoHistory.addVideo(new VideoData(Url, Title, Author,ImageUri, Duration),
                Data.Persistent.MaxVideoHistoryLen);
        }


        private async void SetMetaDataAsync(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                string id = YTManager.getIdFromUrl(url);

                if (!String.IsNullOrWhiteSpace(id))
                {
                    List<YoutubeSearch.VideoInformation> infos;
                    try
                    {
                        // object is disposed after call
                        // surpressed exceptions are fully ignored
                        infos = await new YoutubeSearch.VideoSearch(){encoding = Encoding.UTF8}.SearchQueryTaskAsync(id, 1, 0, true);

                        if (infos.Count > 0)
                        {
                            var videoInfo = infos[0];

                            ImageUri = videoInfo.Thumbnail;
                            Title = videoInfo.Title;
                            Duration = videoInfo.Duration;
                            Author = videoInfo.Author;
                        }
                        else
                        {
                            ImageUri = YTManager.getUrlToThumbnail(url);
                            Title = await YTManager.GetTitleAsync(url);
                        }
                    }
                    catch
                    {
                        //fallback, if rate limit blocks call
                        ImageUri = YTManager.getUrlToThumbnail(url);
                        Title = await YTManager.GetTitleAsync(url);
                    }
                    //prepares the download to reduce later waiting time
                    await DownloadManager.prepareCacheVideoAsync(Url, Title);
                }
                else
                {
                    var info = await YTManager.GetGeneralInfoAsync(url);

                    Title = info.Title;

                    if (info is VideoDownloadInfo vInfo)
                    {
                        ImageUri = vInfo.Thumbnail;
                        Author = vInfo.Uploader;

                        if (vInfo.Duration.HasValue && vInfo.Duration.Value < TimeSpan.MaxValue.TotalSeconds)
                        {
                            TimeSpan t = TimeSpan.FromSeconds(vInfo.Duration.Value);
                            Duration = t.ToString(@"mm\:ss");
                        }

                    }
                }         
            }
        }


        private void box_link_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
                Url = box.Text;
        }


        private void ProcessEntry()
        {
            if (Url.StartsWith("http://") || Url.StartsWith("https://"))
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

            List<YoutubeSearch.VideoInformation> result;
            var ytSearch = new YoutubeSearch.VideoSearch() { encoding = Encoding.UTF8 };

            try
            {
                result = await ytSearch.SearchQueryTaskAsync(filter, pages, 0, true);
            }
            catch(YoutubeSearch.OutdatedPatternException ex)
            {
                Util.IO.LogManager.LogException(ex, "StreamModule/Streammode", "Youtubesearch package is outdated");
                SnackbarManager.SnackbarMessage("Failed to load search result. A dependency might be outdated", SnackbarManager.SnackbarAction.None);
                return;
            }
            catch (Exception ex)
            {
                Util.IO.LogManager.LogException(ex, "StreamModule/StreamMode", "Could not accomplish search for video");
                SnackbarManager.SnackbarMessage("Could not load search result", SnackbarManager.SnackbarAction.None);
                return;
            }



            // filter out unwanted livestreams
            result = result.Where(v => !v.IsLivestream).ToList();

            foreach (var item in result)
            {
                Suggestions.Add(new VideoData(item));
            }

            // show (non-vital) parsing exceptions as a hint for potentially missing videos
            var exCount = ytSearch.exceptionQueue.Count;
            if (exCount > 0)
            {
                var exampleException = ytSearch.exceptionQueue.Dequeue();

                Util.IO.LogManager.LogException(exampleException, "StreamModule/Streammode", "Youtubesearch package is outdated and missing " + exCount.ToString() + " search results");
                SnackbarManager.SnackbarMessage("Failed to load some search results. A dependency might be outdated", SnackbarManager.SnackbarAction.None);
                ytSearch.exceptionQueue.Clear();
            }
        }


        private async void GetAndStartStream(string url, bool IsQueue = false)
        {
            if (url == null)
                return;

            card_downProgress.Visibility = Visibility.Visible;

            DownloadManager.CacheResult result;
            //cache the file using cacheVideoAsync
            try
            {
                result = await DownloadManager.cacheVideoAsync(url, Title);
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "StreamModule/StreamMode.xaml.cs", "The libvideo Regex Function couldn't match the download uri");
                SnackbarManager.SnackbarMessage("Could not resolve video URI", SnackbarManager.SnackbarAction.Log);
                return;
            }
            finally
            {
                card_downProgress.Visibility = Visibility.Collapsed;
            }

            if (!String.IsNullOrWhiteSpace(result.location) || !String.IsNullOrWhiteSpace(result.uri))
            {
                BotData data = new BotData(Title, result.location, result.uri, "", Author);

                if (IsQueue)
                    QueueStream(data);
                else
                    StartStream(data);
            }
            else
            {
                SnackbarManager.SnackbarMessage("Source not supported");
            }
        }

        #region events



        private void Recommended_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //intercept scroll event and make scroll viewer accept the wheel
            ScrollViewer scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 4);
            e.Handled = true;
        }

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

        private void context_addQueue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                if(fe.Tag is string uri)
                {
                    Url = uri;
                    ProcessEntry();
                    GetAndStartStream(uri, true);
                }

                
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