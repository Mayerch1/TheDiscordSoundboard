using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicsordBot
{
    [Serializable()]
    public class PersistentData
    {
        #region persistend vars

        public bool IsFirstStart { get; set; }
        public string SettingsPath { get; set; }
        public int HighestButtonToSave { get; set; }

        public ulong ClientId { get; set; }

        public string Token { get; set; }

        public int VisibleButtons { get; set; }

        #endregion persistend vars

        #region embedded classes

        public List<ButtonData> BtnList { get; set; } = new List<ButtonData>();

        #endregion embedded classes

        //all other settings go here
    }
}