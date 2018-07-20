using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot
{
    public class RuntimeData
    {
        #region constants

        //PUBLISH: remove
        private const string saveFile = "\\Settings.xml";

        #endregion constants

        #region volatile vars

        public List<List<SocketVoiceChannel>> channelList;
        public List<List<SocketGuildUser>> clientList;

        #endregion volatile vars

        #region embedded classes

        public PersistentData persistent { get; set; }

        #endregion embedded classes

        public RuntimeData()
        {
            persistent = new PersistentData();
        }

        ~RuntimeData()
        {
            saveData(saveFile);
        }

        #region ManageData

        //delete empty List elements
        //adds new elements, if more are to be displayed
        public int resizeBtnList()
        {
            //downsize to minimum
            if (persistent.BtnList.Count > persistent.VisibleButtons)
            {
                cleanBtnList();
            }
            //upsize again, if list is to short to display
            if (persistent.BtnList.Count < persistent.VisibleButtons)
            {
                for (int i = persistent.BtnList.Count; i < persistent.VisibleButtons; i++)
                {
                    persistent.BtnList.Add(mkDefaultButtonData());
                }
            }

            return persistent.BtnList.Count;
        }

        //remove all elements above the last with content
        public void cleanBtnList()
        {
            int upper = persistent.BtnList.Count;

            for (int i = persistent.HighestButtonToSave + 1; i < upper; i++)
            {
                persistent.BtnList.RemoveAt(persistent.BtnList.Count - 1);
            }
        }

        #endregion ManageData

        #region Handle Save/Load

        //load all data from the file
        public void loadData(string _file = saveFile)
        {
            persistent.SettingsPath = Properties.Settings.Default.Path;

            if (persistent.SettingsPath == null || persistent.SettingsPath == "")
            {
                persistent.SettingsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\DiscordBot";
            }

            if (System.IO.File.Exists(persistent.SettingsPath + _file))
            {
                try
                {
                    System.IO.StreamReader file = System.IO.File.OpenText(persistent.SettingsPath + saveFile);
                    Type settType = persistent.GetType();
                    System.Xml.Serialization.XmlSerializer xmlSerial = new System.Xml.Serialization.XmlSerializer(settType);
                    object oData = xmlSerial.Deserialize(file);

                    persistent = (PersistentData)oData;
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
        }

        //set all values to the default settings
        private void loadDefaultValues()
        {
            /*---------Persistent-------*/
            //IDEA: adjust visible buttons
            persistent.VisibleButtons = 80;
            persistent.HighestButtonToSave = -1;
            persistent.IsFirstStart = true;
            persistent.Token = null;
            /* all future settings are init. here */

            /*-----------Buttons-------*/

            ButtonData.ForegroundBrush = "#FF707070";
            ButtonData.FontFamily = "Segoe UI";

            //init the visible Buttons
            for (int i = 0; i < persistent.VisibleButtons; i++)
            {
                persistent.BtnList.Add(mkDefaultButtonData());
            }
        }

        //create a ButtonData with default values
        private ButtonData mkDefaultButtonData()
        {
            ButtonData btnD = new ButtonData();

            btnD.File = null;
            btnD.Name = null;
            btnD.Volume = -1;
            btnD.IsLoop = false;
            btnD.BorderBrush = "#FFDDDDDD";
            btnD.BackgroundBrush = "#FF000000";
            btnD.ID = persistent.BtnList.Count;

            return btnD;
        }

        //save all data to file
        public void saveData(string _file = saveFile)
        {
            Properties.Settings.Default.Path = persistent.SettingsPath;
            Properties.Settings.Default.Save();

            cleanBtnList();

            Type settingsType = persistent.GetType();

            System.IO.StreamWriter file;
            try
            {
                file = System.IO.File.CreateText(persistent.SettingsPath + _file);
            }
            catch
            {
                return;
            }
            if (settingsType.IsSerializable)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(settingsType);
                serializer.Serialize(file, persistent);
                file.Flush();
                file.Close();
            }
        }

        #endregion Handle Save/Load
    }
}