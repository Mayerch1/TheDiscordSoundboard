using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot.Data
{
    public class RuntimeData

    {
        #region constants

        private const string saveFile = "\\Settings.xml";

        #endregion constants

        #region volatile vars

        public List<List<SocketVoiceChannel>> channelList;
        public List<List<SocketGuildUser>> clientList;

        #endregion volatile vars

        #region embedded classes

        public PersistentData Persistent { get; set; }

        #endregion embedded classes

        public RuntimeData()
        {
            Persistent = new PersistentData();
        }

        #region ManageData

        //sets HighestButtonToSave to new high
        public void ButtonChanged(int index)
        {
            if (index > Persistent.HighestButtonToSave)
                Persistent.HighestButtonToSave = index;
        }

        //sets HighestButtonToSave if button was reset
        //this must be called AFTER the button was reset
        public void ButtonReset(int index)
        {
            if (index == Persistent.HighestButtonToSave)
            {
                for (int i = index; i >= 0; i--)
                {
                    if (Persistent.BtnList[i].Name != null)
                    {
                        Persistent.HighestButtonToSave = (i - 1);
                        break;
                    }
                }
            }
        }

        //delete empty List elements
        //adds new elements, if more are to be displayed
        public int resizeBtnList()
        {
            //downsize to minimum
            if (Persistent.BtnList.Count > Persistent.VisibleButtons)
            {
                cleanBtnList();
            }
            //upsize again, if list is to short to display
            if (Persistent.BtnList.Count < Persistent.VisibleButtons)
            {
                for (int i = Persistent.BtnList.Count; i < Persistent.VisibleButtons; i++)
                {
                    Persistent.BtnList.Add(mkDefaultButtonData());
                }
            }

            return Persistent.BtnList.Count;
        }

        //remove all elements above the last with content
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

        //load all data from the file
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
        }

        //set all values to the default settings
        private void loadDefaultValues()
        {
            /*---------Persistent-------*/

            /* all future settings are init. here */

            /*-----------Buttons-------*/

            //init the visible Buttons
            for (int i = 0; i < Persistent.VisibleButtons; i++)
            {
                Persistent.BtnList.Add(mkDefaultButtonData());
            }
        }

        //create a ButtonData with default values
        private ButtonData mkDefaultButtonData()
        {
            ButtonData btnD = new ButtonData();

            btnD.ID = Persistent.BtnList.Count;

            return btnD;
        }

        //save all data to file
        public void saveData(string _file = saveFile)
        {
            Properties.Settings.Default.Path = Persistent.SettingsPath;

            Properties.Settings.Default.Save();

            cleanBtnList();

            Type settingsType = Persistent.GetType();

            System.IO.Directory.CreateDirectory(Persistent.SettingsPath);

            System.IO.StreamWriter file;
            try
            {
                file = System.IO.File.CreateText(Persistent.SettingsPath + _file);
            }
            catch (Exception ex)
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
    }
}