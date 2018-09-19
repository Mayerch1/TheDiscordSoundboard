using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DicsordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for ButtonHotkeyWindow.xaml
    /// </summary>
    ///
    public partial class ButtonHotkeyWindow : Window
    {
        private Data.ButtonData Btn { get { return Handle.Data.Persistent.BtnList[btnIndex]; } set { Handle.Data.Persistent.BtnList[btnIndex] = value; } }
        private int btnIndex = 0;

        private Data.Hotkey Hotkey { get { return Handle.Data.Persistent.HotkeyList[hotkeyIndex]; } set { Handle.Data.Persistent.HotkeyList[hotkeyIndex] = value; } }
        private int hotkeyIndex = 0;
        private int BtnId { get; set; }

        private uint vk_code = 0;

        public ButtonHotkeyWindow(int btnId, double x, double y, double pWidth, double pHeight)
        {
            BtnId = btnId;

            //get hotkeyIndex
            for (int i = 0; i < Handle.Data.Persistent.HotkeyList.Count; i++)
            {
                if (Handle.Data.Persistent.HotkeyList[i].btn_id == BtnId)
                {
                    hotkeyIndex = i;
                    break;
                }
            }

            //get btnIndex
            for (int i = 0; i < Handle.Data.Persistent.BtnList.Count; i++)
            {
                if (Handle.Data.Persistent.BtnList[i].ID == BtnId)
                {
                    btnIndex = i;
                    break;
                }
            }

            InitializeComponent();
            double finalX = (x + pWidth / 2) - (this.Width / 2);
            double finalY = (y + pHeight / 2) - (this.Height / 2);
            this.Left = finalX;
            this.Top = finalY;

            box_Hotkey.Text = KeyInterop.KeyFromVirtualKey((int)Hotkey.vk_code).ToString();
            setModifierCheckBoxes(Hotkey.mod_code);
        }

        private void setModifierCheckBoxes(uint mod)
        {
            var modifiers = HotkeyManager.getBoolFromCode(mod);

            box_shift.IsChecked = modifiers.Item1;
            box_ctrl.IsChecked = modifiers.Item2;
            box_win.IsChecked = modifiers.Item3;
            box_win.IsChecked = modifiers.Item4;
        }

        private uint getModifierCheckBoxes()
        {
            return HotkeyManager.getCodeFromBool((bool)box_shift.IsChecked, (bool)box_ctrl.IsChecked, (bool)box_win.IsChecked, (bool)box_alt.IsChecked);
        }

        private void saveHotkey()
        {
            //save into Handle.data.Persistent.Hotkeys
            Hotkey.mod_code = getModifierCheckBoxes();
            Hotkey.vk_code = vk_code;

            //save into Handle.Data.Persistent.BtnList
            Btn.Hotkey_MOD = Hotkey.mod_code;
            Btn.Hotkey_VK = vk_code;
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            saveHotkey();
            DialogResult = true;

            this.Close();
        }

        private void box_Hotkey_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: save keycode and update checkboxes on btn press
            //if (e.Key != Key.LeftAlt && e.Key != Key.LWin && e.Key != Key.LeftAlt)
            //{
            //    Btn.Hotkey_VK = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            //    Console.WriteLine(Btn.Hotkey_VK);
            //}
        }
    }

#pragma warning restore CS1591
}