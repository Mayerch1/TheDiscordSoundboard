using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot.Data
{
    /// <summary>
    /// Hotkey class, contains basic information about a hotke
    /// </summary>
    public class Hotkey
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Hotkey()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_keyCode">keycode</param>
        /// <param name="_modifier">modifier code</param>
        public Hotkey(uint _keyCode, uint _modifier)
        {
            vk_code = _keyCode;
            mod_code = _modifier;
        }

        /// <summary>
        /// virtual keycode of hotkey
        /// </summary>
        public uint vk_code = 0;

        /// <summary>
        /// modifier code of hotkey
        /// </summary>
        public uint mod_code = 0;

        /// <summary>
        /// if -1, no button is assigned
        /// </summary>
        public int btn_id = -1;
    }
}