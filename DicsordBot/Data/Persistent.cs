using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot.Data
{
    [Serializable()]
    public class PersistentData : INotifyPropertyChanged
    {
        #region consts

        public const float earrapeValue = 10f;

        #endregion consts

        #region persistend fields

        private bool isFirstStart = true;
        private string settingsPath;
        private int highestButtonToSave = -1;
        private ulong clientId;
        private string clientName;
        private ulong channelId = 0;
        private string clientAvatar;
        private string token = null;
        private int minVisibleButtons = 36;
        private float volume = 0.5f;
        private float volumeCap = 0.3f;

        #endregion persistend fields

        #region persistend properties

        public bool IsFirstStart { get { return isFirstStart; } set { isFirstStart = value; OnPropertyChanged("IsFirstStart"); } }
        public string SettingsPath { get { return settingsPath; } set { settingsPath = value; OnPropertyChanged("SettingsPath"); } }
        public int HighestButtonToSave { get { return highestButtonToSave; } set { highestButtonToSave = value; OnPropertyChanged("HighestButtonToSave"); } }

        public ulong ClientId { get { return clientId; } set { clientId = value; OnPropertyChanged("ClientId"); } }

        //set to 0 to join to owners channel
        public ulong ChannelId { get { return channelId; } set { channelId = value; OnPropertyChanged("ChannelId"); } }

        public string ClientAvatar { get { return clientAvatar; } set { clientAvatar = value; OnPropertyChanged("ClientAvatar"); } }
        public string ClientName { get { return clientName; } set { clientName = value; OnPropertyChanged("ClientName"); } }

        public string Token { get { return token; } set { token = value; OnPropertyChanged("Token"); } }

        public int MinVisibleButtons { get { return minVisibleButtons; } set { minVisibleButtons = value; OnPropertyChanged("VisibleButtons"); } }

        public float Volume { get { return volume; } set { volume = value; OnPropertyChanged("Volume"); } }

        public float VolumeCap { get { return volumeCap; } set { volumeCap = value; OnPropertyChanged("VolumeCap"); } }

        #endregion persistend properties

        #region embedded classes

        public List<ButtonData> BtnList { get; set; } = new List<ButtonData>();

        #endregion embedded classes

        //all other settings go here

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }
    }
}