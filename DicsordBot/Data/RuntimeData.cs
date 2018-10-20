﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DiscordBot.Data
{
    /// <summary>
    /// contains runtime data + class for persistent data, handles and manages those data
    /// </summary>
    public class RuntimeData : INotifyPropertyChanged

    {
        #region constants

        private const string applicationDirectory = "\\DiscordSoundboard";
        private const string saveFile = "\\Settings.xml";
        private const string fileFile = "\\Files.xml";
        private const string playlistFile = "\\Playlist.xml";
        private const string historyFile = "\\History.xml";

        #endregion constants

        #region fields

        private PersistentData persistent = new PersistentData();

        private ObservableCollection<FileData> files = new ObservableCollection<FileData>();
        private ObservableCollection<Playlist> playlists = new ObservableCollection<Playlist>();
        private History history = new History();
        private int playlistIndex = 0;
        private int playlistFileIndex = 0;
        private bool isPlaylistPlaying = false;

        #endregion fields

        #region properties

        /// <summary>
        /// Persistent property (class)
        /// </summary>
        public PersistentData Persistent { get { return persistent; } set { persistent = value; OnPropertyChanged("Persistent"); } }

        /// <summary>
        /// Files property (collection of classes)
        /// </summary>
        public ObservableCollection<FileData> Files { get { return files; } set { files = value; OnPropertyChanged("Files"); } }

        /// <summary>
        /// List of all playlists
        /// </summary>
        public ObservableCollection<Playlist> Playlists { get { return playlists; } set { playlists = value; OnPropertyChanged("PlayLists"); } }

        /// <summary>
        /// History of played files
        /// </summary>
        public History History { get { return history; } set { history = value; OnPropertyChanged("History"); } }

        /// <summary>
        /// index of currently played playlist
        /// </summary>
        public int PlaylistIndex { get { return playlistIndex; } set { playlistIndex = value; OnPropertyChanged("PlaylistIndex"); } }

        /// <summary>
        /// file index of position in playlist
        /// </summary>
        public int PlaylistFileIndex { get { return playlistFileIndex; } set { playlistFileIndex = value; OnPropertyChanged("PlaylistFileIndex"); } }

        /// <summary>
        /// IsPlaylistPlaying property
        /// </summary>
        public bool IsPlaylistPlaying { get { return isPlaylistPlaying; } set { isPlaylistPlaying = value; OnPropertyChanged("IsPlaylistPlaying"); } }

        #endregion properties

        /// <summary>
        /// constructor of class
        /// </summary>
        public RuntimeData()
        {
        }

        #region ManageData

        /// <summary>
        /// Handler, called if any Button property has changed
        /// </summary>
        /// <param name="sender">sender of event, not used</param>
        /// <param name="e">EventArgs, not used</param>
        private void HandleButtonPropertyChanged(object sender, EventArgs e)
        {
            determinHighestButton();
        }

        /// <summary>
        /// gets the highest button which is to be saved and displayed + 1
        /// </summary>
        /// <remarks>
        /// based on minVisibleButtons and the highest Button Nr which is not empty
        /// </remarks>
        private void determinHighestButton()
        {
            Persistent.HighestButtonToSave = -1;

            foreach (var element in Persistent.BtnList)
            {
                //a button with file and name null is considered empty
                if (!String.IsNullOrEmpty(element.Name) && !String.IsNullOrEmpty(element.File))
                    Persistent.HighestButtonToSave = element.ID;
            }
            OnPropertyChanged("BtnList");
        }

        /// <summary>
        /// shortens or lengthens buttonList based on the highest valid button
        /// </summary>
        /// <returns>new lenght of the buttonList</returns>
        /// <see cref="determinHighestButton()"/>
        public int resizeBtnList()
        {
            determinHighestButton();
            //downsize to minimum
            cleanBtnList();

            //upsize again, if list is to short to display
            if (Persistent.BtnList.Count < PersistentData.minVisibleButtons || Persistent.BtnList.Count < (Persistent.HighestButtonToSave + 2))
            {
                //use highest from both if values above (short if)
                int highestBtn = PersistentData.minVisibleButtons > (Persistent.HighestButtonToSave + 2) ? PersistentData.minVisibleButtons : (Persistent.HighestButtonToSave + 2);

                for (int i = Persistent.BtnList.Count; i < highestBtn; i++)
                {
                    Persistent.BtnList.Add(mkDefaultButtonData());
                }
            }

            return Persistent.BtnList.Count;
        }

        /// <summary>
        /// shortens button list to the minimum without losing data
        /// </summary>
        /// <remarks>
        /// disregards minVisibleButtons
        /// </remarks>
        private void cleanBtnList()
        {
            int upper = Persistent.BtnList.Count;

            for (int i = Persistent.HighestButtonToSave + 1; i < upper; i++)
            {
                Persistent.BtnList.RemoveAt(Persistent.BtnList.Count - 1);
            }
        }

        #endregion ManageData

        #region Handle Save/Load

        /// <summary>
        /// loads all Propertys of RuntimeData from different xml files
        /// </summary>
        public void loadAll()
        {
            try
            {
                Persistent.SettingsPath = Properties.Settings.Default.Path;
            }
            catch
            {
                Persistent.SettingsPath = null;
            }

            if (String.IsNullOrWhiteSpace(Persistent.SettingsPath))
                Persistent.SettingsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + applicationDirectory;

            if ((Persistent = (PersistentData)loadObject(Persistent, saveFile)) == null)
            {
                //create new one, old one was overwirtten with =null
                Persistent = new PersistentData();
                //Persistent=new... destroyed this information
                Persistent.SettingsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + applicationDirectory;
                loadDefaultValues();
            }
            //normalize btn
            resizeBtnList();

            if ((Files = (ObservableCollection<FileData>)loadObject(Files, fileFile)) == null)
                Files = new ObservableCollection<FileData>();

            if ((Playlists = (ObservableCollection<Playlist>)loadObject(Playlists, playlistFile)) == null)
                Playlists = new ObservableCollection<Playlist>();

            if ((History = (History)loadObject(History, historyFile)) == null)
                History = new History();
        }

        /// <summary>
        /// deserialises a specific object, passed as parameter
        /// </summary>
        /// <param name="target">object to load</param>
        /// <param name="_file">path to file, including name</param>
        /// <returns>returns null on failure</returns>
        private object loadObject(object target, string _file)
        {
            //load persistent data
            if (System.IO.File.Exists(Persistent.SettingsPath + _file))
            {
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(Persistent.SettingsPath + _file);
                    Type fileType = target.GetType();

                    System.Xml.Serialization.XmlSerializer xmlSerial = new System.Xml.Serialization.XmlSerializer(fileType);
                    target = xmlSerial.Deserialize(file);
                    file.Close();
                    return target;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// load default value for all settings, if no saves are available
        /// </summary>
        private void loadDefaultValues()
        {
            //init the visible Buttons
            for (int i = 0; i < PersistentData.minVisibleButtons; i++)
            {
                Persistent.BtnList.Add(mkDefaultButtonData());
            }
            //init media sources
            Persistent.MediaSources.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
        }

        /// <summary>
        /// creates one default ButtonData object
        /// </summary>
        /// <returns>ButtonData object with default values and correct id</returns>
        private ButtonData mkDefaultButtonData()
        {
            ButtonData btnD = new ButtonData();

            btnD.ID = Persistent.BtnList.Count;

            return btnD;
        }

        /// <summary>
        /// saves all property of RuntimeData in different files
        /// </summary>
        public void saveAll()
        {
            try
            {
                Properties.Settings.Default.Path = Persistent.SettingsPath;
            }
            catch {/* do nothing */}

            Properties.Settings.Default.Save();

            cleanBtnList();
            saveObject(Persistent, saveFile);
            saveObject(Files, fileFile);
            saveObject(Playlists, playlistFile);
            saveObject(History, historyFile);
        }

        /// <summary>
        /// serializes an object and saves it into an file
        /// </summary>
        /// <param name="target">object to serialize</param>
        /// <param name="_file">path for saving, including file name</param>
        private void saveObject(object target, string _file)
        {
            Type fileType = target.GetType();

            //test for existing dir
            if (!String.IsNullOrEmpty(Persistent.SettingsPath))
            {
                System.IO.Directory.CreateDirectory(Persistent.SettingsPath);
            }

            System.IO.StreamWriter file;
            try
            {
                file = System.IO.File.CreateText(Persistent.SettingsPath + _file);
            }
            catch (Exception)
            {
                return;
            }
            //if (fileType.IsSerializable)
            //{
            //serialize
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(fileType);
            serializer.Serialize(file, target);
            file.Flush();
            file.Close();
            //}
        }

        #endregion Handle Save/Load

        #region events

        /// <summary>
        /// PropertyChanged Event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// propertychanged method, notifies the actual handler
        /// </summary>
        /// <param name="info"></param>
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

        #endregion events
    }
}