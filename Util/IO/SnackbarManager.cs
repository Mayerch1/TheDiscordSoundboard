using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.IO
{
    public static class SnackbarManager
    {
        /// <summary>
        /// SnackbarAction enum, passed into delegate for informing eventHandler on requested action
        /// </summary>
        public enum SnackbarAction
        {
            /// <summary>
            /// Open the Settings menu, when clicked
            /// </summary>
            Settings,

            /// <summary>
            /// Open the update page, when clicked
            /// </summary>
            Update,

            /// <summary>
            /// Open the log file, to see more details
            /// </summary>
            Log,

            /// <summary>
            /// Do nothing on click
            /// </summary>
            None
        };


        /// <summary>
        /// SnackbarWarningHandle
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="action">Enum for triggered action in shown Snackbar</param>
        public delegate void SnackbarMessageHandle(string msg, SnackbarAction action = SnackbarAction.None);

        /// <summary>
        /// SnackbarWarning instance
        /// </summary>
        public static SnackbarMessageHandle SnackbarMessage;
     
    }
}
