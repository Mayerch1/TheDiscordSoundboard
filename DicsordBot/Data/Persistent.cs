using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot.Data
{
    [Serializable()]
    public class PersistentData
    {
        #region persistend vars

        public bool IsFirstStart { get; set; } = true;
        public string SettingsPath { get; set; }
        public int HighestButtonToSave { get; set; } = -1;

        public ulong ClientId { get; set; }

        public string Token { get; set; } = null;

        public int VisibleButtons { get; set; } = 36;

        public float Volume { get; set; } = 0.5f;

        #endregion persistend vars

        #region embedded classes

        public List<ButtonData> BtnList { get; set; } = new List<ButtonData>();

        #endregion embedded classes

        //all other settings go here
    }
}