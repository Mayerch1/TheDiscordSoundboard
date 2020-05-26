using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Controls;
using DataManagement;
using Microsoft.Win32;

using RestSharp;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Models.Bot;

namespace DiscordBot.UI
{
    /// <summary>
    /// Interaction logic for ButtonUI.xaml
    /// </summary>
    public partial class ButtonUI : INotifyPropertyChanged
    {
#pragma warning disable CS1591

        private ObservableCollection<DataManagement.ButtonData> btnList = new ObservableCollection<DataManagement.ButtonData>();

        public ObservableCollection<DataManagement.ButtonData> BtnList
        {
            get => btnList;
            set { btnList = value; OnPropertyChanged("BtnList"); }
        }


        public delegate void InstantButtonClickedHandler(BotTrackData track);

        public InstantButtonClickedHandler InstantButtonClicked;

        public delegate void ToggleHotkeyHandler(bool isEnabled);

        public ToggleHotkeyHandler ToggleHotkey;

        public ButtonUI()
        {
            //TODO: move to async
            var rest = RestStorage.GetClient();
            var request = RestStorage.GetRequest("Buttons", Method.GET);
            var resp = rest.Get(request);

            BtnList = RestStorage.JsonToObj<ObservableCollection<DataManagement.ButtonData>>(resp.Content);
            if (BtnList == null)
            {
                BtnList = new ObservableCollection<ButtonData>();
            }

            BtnList.Add(new ButtonData());
            
            DataManagement.ButtonData.Width = 170;
            DataManagement.ButtonData.Height = 80;

            InitializeComponent();

            Handle.Data.resizeBtnList();

            this.DataContext = this;
            btnControl.ItemsSource = BtnList;

            // clear event handler and add this single one
            ButtonData.ButtonUpdated = null;
            ButtonData.ButtonUpdated += OnButtonUpdate;
        }


        ~ButtonUI()
        {
            // cleanup the references to handler
            ButtonData.ButtonUpdated -= OnButtonUpdate;
        }


        /// <summary>
        /// add the new button into the dtabase
        /// </summary>
        /// <param name="created">button to add</param>
        /// <returns>new created ButtonData, can have different Id than input</returns>
        private Buttons OnButtonCreation(Buttons created)
        {
            if(created.TrackId != 0)
            {
                // only send index to database, not the resolved field
                created.Track = null;
            }
            
            var rest = RestStorage.GetClient();
            var request = RestStorage.GetRequest("Buttons", Method.POST);
            request.AddJsonBody(created);
            var resp = rest.Post(request);

            

            Buttons btn = RestStorage.JsonToObj<Buttons>(resp.Content);
            return btn;
        }


        private long OnButtonUpdate(Buttons update)
        {
            

            var rest = RestStorage.GetClient();
            var request = RestStorage.GetRequest("Buttons/" + update.Id, Method.PUT);            
            request.AddJsonBody(update);

            var resp = rest.Put(request);

            // add new empty button if that was the last one
            if(update == BtnList[BtnList.Count - 1])
            {
                BtnList.Add(new ButtonData());
            }
            
            // insert newly created button
            if(resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return OnButtonCreation(update).Id;
            }
            return update.Id;
        }


        private BotTrackData ButtonToBotTrackData(Buttons btn)
        {
            BotTrackData data = new BotTrackData();
            data.Track = btn.Track;
            data.Metadata.IsEarrape = btn.IsEarrape;
            data.Metadata.IsLoop = btn.IsLoop;

            return data;    
        }

        private void btn_Instant_Click(object sender, RoutedEventArgs e)
        {
            //event is handled in MainWindow

            if (sender is FrameworkElement fe)
            {
                if (fe.Tag != null && (long)fe.Tag != 0)
                {
                    Buttons tgtBtn = BtnList.First(x => x.Id == (long)fe.Tag);
                    var data = ButtonToBotTrackData(tgtBtn);
                    data.Metadata.ForceReplay = true; // instant replay

                    InstantButtonClicked(data);
                }
                    
            }
        }

        private void btn_Queue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.Tag != null && (long)fe.Tag != 0)
                {
                    Buttons tgtBtn = BtnList.First(x => x.Id == (long)fe.Tag);
                    var data = ButtonToBotTrackData(tgtBtn);
                    data.Metadata.ForceReplay = false; // add to queue

                    InstantButtonClicked(data);
                }
            }
        }


        private void btn_FileChooser_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int id = (int)(long)btn.Tag;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            string allFormats = "*" + Handle.Data.Persistent.SupportedFormats.Aggregate((i, j) => i + ";" + "*" + j);
            string allFormatString = "all supported types |" + allFormats;

            openFileDialog.Filter = allFormatString +
                                    "|mp3/wav files (*.mp3/*.wav)|*.mp3;*.wav" +
                                    "|all files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true && openFileDialog.CheckFileExists)
            {
                // use base class to prevent automatic updates while editing
                Buttons tgtBtn = null;
                
                if(id == 0)
                {
                    // this is the empty dummy-Button
                    tgtBtn = BtnList[BtnList.Count - 1];
                }
                else
                {
                    tgtBtn = BtnList.First(x => x.Id == id);
                }
                
                

                // check if the file exists in the database
                var rest = RestStorage.GetClient();
                var req = RestStorage.GetRequest("TrackData", Method.GET);
                req.AddQueryParameter("LocalFile", openFileDialog.FileName);
                var resp = rest.Get(req);
                var matches = RestStorage.JsonToObj<List<TrackData>>(resp.Content);


                // file is not in db yet
                if(matches == null || matches.Count == 0)
                {
                    //TODO: have global class for creating new TrackData from disk
                    var info = Util.IO.FileWatcher.getAllFileInfo(openFileDialog.FileName);

                    // use base object to not update db on every field change
                    // do maunal update once all fields are updated
                    TrackData tData = new TrackData();

                    tData.LocalFile = openFileDialog.FileName;
                    tData.Author = info.Author;
                    tData.Album = info.Album;
                    tData.Genre = info.Genre;
                    tData.Name = evaluateName(tData.LocalFile);

                    // save the file into the db
                    req = RestStorage.GetRequest("TrackData", Method.POST);
                    req.AddJsonBody(tData);
                    resp = rest.Post(req);

                    // the post might change the id of the track
                    tData = RestStorage.JsonToObj<TrackData>(resp.Content);
                    tgtBtn.Track = tData;
                    tgtBtn.TrackId = tData.Id;
                }
                // file is existing in db
                else
                {
                    // all files matching are eqiavalent
                    tgtBtn.TrackId = matches[0].Id;
                    tgtBtn.Track = matches[0];
                }

                

                // only set nickname if not set yet
                if (tgtBtn.NickName == null)
                {
                    tgtBtn.NickName = tgtBtn.Track.Name;
                }

                // manual update to prevent update on every property update
                OnButtonUpdate(tgtBtn);
                // notify ui of changes (as Signal was blocked before)
                ((ButtonData)tgtBtn).NotifyNameChanged();
            }
        }

        private void btn_HotkeyEditor_Click(object sender, RoutedEventArgs e)
        {
            int tag = (int)((FrameworkElement)sender).Tag;

            //disable hotkeys, while editing them
            ToggleHotkey(false);
            //trigger blur effect
            Util.IO.BlurEffectManager.ToggleBlurEffect(true);

            var popup = new ButtonHotkeyPopup(tag, Application.Current.MainWindow);
            popup.IsOpen = true;

            popup.Closed += delegate (object pSender, EventArgs pE)
            {
                Util.IO.BlurEffectManager.ToggleBlurEffect(false);
                ToggleHotkey(true);
            };
        }

        //return only the file name from a Path to a file
        private string evaluateName(string filePath)
        {
            var fileType = System.IO.Path.GetFileName(filePath);
            var fileName = fileType.Substring(0, fileType.LastIndexOf('.'));

            return fileName;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        private void btn_Return_Click(object sender, RoutedEventArgs e)
        {
            Handle.Data.resizeBtnList();
        }

     

#pragma warning restore CS1591
    }
}