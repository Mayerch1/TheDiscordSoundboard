using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace DicsordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for ButtonHotkeyPopup.xaml
    /// </summary>
    public partial class ButtonHotkeyPopup : Popup
    {
        private Data.ButtonData Btn { get { return Handle.Data.Persistent.BtnList[btnIndex]; } set { Handle.Data.Persistent.BtnList[btnIndex] = value; } }
        private int btnIndex = 0;

        private Data.Hotkey Hotkey { get { return Handle.Data.Persistent.HotkeyList[hotkeyIndex]; } set { Handle.Data.Persistent.HotkeyList[hotkeyIndex] = value; } }
        private int hotkeyIndex = -1;
        private int BtnId { get; set; }

        private uint vk_code = 0;
        private uint mod_code = 0;

        public ButtonHotkeyPopup(int btnId, Window window)
        {
            BtnId = btnId;
            //get btnIndex
            for (int i = 0; i < Handle.Data.Persistent.BtnList.Count; i++)
            {
                if (Handle.Data.Persistent.BtnList[i].ID == BtnId)
                {
                    btnIndex = i;
                    break;
                }
            }

            //get hotkeyIndex
            for (int i = 0; i < Handle.Data.Persistent.HotkeyList.Count; i++)
            {
                if (Handle.Data.Persistent.HotkeyList[i].btn_id == BtnId)
                {
                    hotkeyIndex = i;
                    break;
                }
            }
            if (hotkeyIndex == -1)
            {
                //create new hotkey, set hotkeyIndex to new hotkey
                var newHotkey = new Data.Hotkey()
                {
                    btn_id = BtnId,
                };

                Handle.Data.Persistent.HotkeyList.Add(newHotkey);
                hotkeyIndex = Handle.Data.Persistent.HotkeyList.Count - 1;
            }

            //set namebox and vk variable
            vk_code = Hotkey.vk_code;
            mod_code = Hotkey.mod_code;

            //-------- set location ------------------
            this.PlacementTarget = window;
            this.Placement = PlacementMode.Center;
            this.StaysOpen = false;
            //--------- set window -------------------------
            InitializeComponent();
            Keyboard.Focus(box_Hotkey);

            //------------set boxes and textboxes --------------------------

            box_ButtonInfo.Text = Btn.Name;

            //set button name, if not empty
            box_ButtonInfo.Text = "";
            if (!String.IsNullOrWhiteSpace(Btn.Name))
            {
                box_ButtonInfo.Text += "-" + Btn.Name + "- ";
            }
            //set button number, not! the Id
            box_ButtonInfo.Text += "(Nr. " + (Btn.ID + 1) + ")";

            box_Hotkey.Text = KeyInterop.KeyFromVirtualKey((int)vk_code).ToString();

            box_Hotkey.Focus();

            //set modiifier checkbox
            setModifierCheckBoxes(mod_code);

            //reset warning
            checkForDoubleHotkey(vk_code, mod_code);
        }

        private uint getModifierCheckBoxes()
        {
            return IO.HotkeyManager.getCodeFromBool((bool)box_shift.IsChecked, (bool)box_ctrl.IsChecked, (bool)box_win.IsChecked, (bool)box_alt.IsChecked);
        }

        private void setModifierCheckBoxes(uint mod)
        {
            var modifiers = IO.HotkeyManager.getBoolFromCode(mod);

            box_shift.IsChecked = modifiers.Item1;
            box_ctrl.IsChecked = modifiers.Item2;
            box_win.IsChecked = modifiers.Item3;
            box_alt.IsChecked = modifiers.Item4;
        }

        private void setModifierCheckBoxes(ModifierKeys modifiers)
        {
            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                box_alt.IsChecked = true;
            else
                box_alt.IsChecked = false;

            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                box_shift.IsChecked = true;
            else
                box_shift.IsChecked = false;

            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                box_ctrl.IsChecked = true;
            else
                box_ctrl.IsChecked = false;

            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                box_win.IsChecked = true;
            else
                box_win.IsChecked = false;
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

        private void box_Hotkey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = (uint)KeyInterop.VirtualKeyFromKey(e.Key);
            //if you press f10 the method above returns 0, which is alt at the same time
            if (e.SystemKey == Key.F10)
                keyCode = 0x79;

            if (!isKeyCodeModifier(keyCode))
            {
                vk_code = keyCode;

                //set keycode
                box_Hotkey.Text = "";
                box_Hotkey.Text = KeyInterop.KeyFromVirtualKey((int)vk_code).ToString();

                //set modifier checkboxes
                setModifierCheckBoxes(e.KeyboardDevice.Modifiers);

                mod_code = IO.HotkeyManager.getCodeFromBool(box_shift.IsChecked, box_ctrl.IsChecked, box_win.IsChecked, box_alt.IsChecked);

                //set warning for double hotkey
                checkForDoubleHotkey(vk_code, mod_code);

                e.Handled = true;
            }
        }

        private void checkForDoubleHotkey(uint vk_code, uint mod_code)
        {
            bool isTaken = false;
            int takenId = 0;

            //find same pressed key
            foreach (var shortcut in Handle.Data.Persistent.HotkeyList)
            {
                if ((shortcut.vk_code == vk_code) && (shortcut.mod_code == mod_code))
                {
                    //cant't have conflict with yourself
                    if (shortcut.btn_id != BtnId)
                    {
                        isTaken = true;
                        takenId = shortcut.btn_id;
                        break;
                    }
                }
            }

            if (isTaken)
            {
                stack_warning.Visibility = Visibility.Visible;
                stack_warning_btnId.Text = (takenId + 1).ToString();
            }
            else
                stack_warning.Visibility = Visibility.Hidden;
        }

        private bool isKeyCodeModifier(uint vk_code)
        {
            /*
             * modifiers are:
             * 0xa0 - 0xa4
             * 0x5b - 0x5c
             * 0x00 (this is alt and f10 at the same time ?!
             * 0xdf
             */
            if ((vk_code <= 0xa4 && vk_code >= 0xa0) || (vk_code <= 0x5c && vk_code >= 0x5b) || (vk_code == 0) || (vk_code == 0xdf))
            {
                return true;
            }
            return false;
        }

        private void box_Checked(object sender, RoutedEventArgs e)
        {
            mod_code = IO.HotkeyManager.getCodeFromBool(box_shift.IsChecked, box_ctrl.IsChecked, box_win.IsChecked, box_alt.IsChecked);

            checkForDoubleHotkey(vk_code, mod_code);
        }

        private void btn_clearHotkey_Click(object sender, RoutedEventArgs e)
        {
            vk_code = 0;
            mod_code = 0;
            setModifierCheckBoxes(mod_code);
            box_Hotkey.Text = "None";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                declineChangesClose();
            }
        }

        private void btn_abort(object sender, RoutedEventArgs e)
        {
            declineChangesClose();
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            acceptChangesClose();
        }

        private void acceptChangesClose()
        {
            saveHotkey();

            if (vk_code == 0 && mod_code == 0)
            {
                Handle.Data.Persistent.HotkeyList.RemoveAt(hotkeyIndex);
            }

            this.IsOpen = false;
        }

        private void declineChangesClose()
        {
            if (vk_code == 0 && mod_code == 0)
            {
                Handle.Data.Persistent.HotkeyList.RemoveAt(hotkeyIndex);
            }
            this.IsOpen = false;
        }

        #region NonTopmostPopup

        protected override void OnOpened(EventArgs e)
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            RECT rect;

            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion P/Invoke imports & definitions

        #endregion NonTopmostPopup
    }

#pragma warning restore CS1591
}