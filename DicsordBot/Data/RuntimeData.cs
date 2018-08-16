using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot.Data
{
    /// <summary>
    /// contains runtime data + class for persistent data, handles and manages those data
    /// </summary>
    public class RuntimeData : INotifyPropertyChanged

    {
        #region constants

        /// <summary>
        /// saveFile, Name of file
        /// </summary>
        private const string saveFile = "\\Settings.xml";

        #endregion constants

        #region fields

        private PersistentData persistent = new PersistentData();

        #endregion fields

        #region properties

        /// <summary>
        /// Persistent property (class)
        /// </summary>
        public PersistentData Persistent { get { return persistent; } set { persistent = value; OnPropertyChanged("Persistent"); } }

        #endregion properties

        public RuntimeData()
        {
            //ButtonData.PropertyChanged += HandleButtonPropertyChanged;
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
        public void cleanBtnList()
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
        /// load all data from saved xml, load default if failed
        /// </summary>
        /// <param name="_file">Name of *.xml setting without path</param>
        public void loadData(string _file = saveFile)
        {
            Persistent.SettingsPath = Properties.Settings.Default.Path;

            if (Persistent.SettingsPath == null || Persistent.SettingsPath == "")
            {
                Persistent.SettingsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DiscordBot";
            }

            if (System.IO.File.Exists(Persistent.SettingsPath + _file))
            {
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(Persistent.SettingsPath + saveFile);
                    Type settType = Persistent.GetType();
                    System.Xml.Serialization.XmlSerializer xmlSerial = new System.Xml.Serialization.XmlSerializer(settType);
                    object oData = xmlSerial.Deserialize(file);

                    Persistent = (PersistentData)oData;
                    file.Close();
                }
                catch
                {
                    loadDefaultValues();
                }
            }
            else
            {
                loadDefaultValues();
            }
            resizeBtnList();
        }

        /// <summary>
        /// load default value for all settings
        /// </summary>
        private void loadDefaultValues()
        {
            //init the visible Buttons
            for (int i = 0; i < PersistentData.minVisibleButtons; i++)
            {
                Persistent.BtnList.Add(mkDefaultButtonData());
            }
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
        /// saves all data into an xml file
        /// </summary>
        /// <param name="_file">Name of *.xml setting without path</param>
        public void saveData(string _file = saveFile)
        {
            Properties.Settings.Default.Path = Persistent.SettingsPath;

            Properties.Settings.Default.Save();

            //clear all empty buttons
            cleanBtnList();

            Type settingsType = Persistent.GetType();

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
            if (settingsType.IsSerializable)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(settingsType);
                serializer.Serialize(file, Persistent);
                file.Flush();
                file.Close();
            }
        }

        #endregion Handle Save/Load

        #region events

        public event PropertyChangedEventHandler PropertyChanged;

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