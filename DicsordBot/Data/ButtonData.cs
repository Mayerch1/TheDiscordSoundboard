﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DicsordBot.Data
{
    /// <summary>
    /// Represents on Button with all its properties, implements INotifyPropertyChanged
    /// </summary>
    [Serializable()]
    public class ButtonData : INotifyPropertyChanged

    {
        #region saved fields

        private string name = null;
        private string file = null;
        private bool isEarrape = false;
        private bool isLoop = false;
        private int iD;

        #endregion saved fields

        #region propertys

        /// <summary>
        /// Name property
        /// </summary>
        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        /// <summary>
        /// File property
        /// </summary>
        /// <value>
        /// path towards file in filesystem
        /// </value>
        public string File { get { return file; } set { file = value; OnPropertyChanged("File"); } }

        /// <summary>
        /// IsEarrape property
        /// </summary>
        /// <value>
        /// determins if boost should be applied
        /// </value>
        public bool IsEarrape { get { return isEarrape; } set { isEarrape = value; OnPropertyChanged("IsEarrape"); } }

        /// <summary>
        /// IsLoop property
        /// </summary>
        public bool IsLoop { get { return isLoop; } set { isLoop = value; OnPropertyChanged("IsLoop"); } }

        /// <summary>
        /// ID property
        /// </summary>
        /// <value>
        /// incremental button id, also stored in tag of btn to assign ui-button to data-button
        /// </value>
        public int ID { get { return iD; } set { iD = value; OnPropertyChanged("ID"); } }

        #endregion propertys

        #region event

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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion event
    }
}