using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DicsordBot.IO
{
    //This is for testing purposes only
    /// <summary>
    /// HotkeyManager class
    /// </summary>
    public static class HotkeyManager
    {
        /// <summary>
        /// delegate for hotkey event
        /// </summary>
        /// <param name="lParam">lParam with keycode and modifiers</param>
        public delegate void RegisterdHotkeyHandle(IntPtr lParam);

        /// <summary>
        /// triggered when registered hotkey was pressed
        /// </summary>
        public static RegisterdHotkeyHandle RegisteredHotkeyPressed;

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
    [In] IntPtr hWnd,
    [In] int id,
    [In] uint fsModifiers,
    [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private static HwndSource _source;
        private const int HOTKEY_ID = 9000;

        /// <summary>
        /// init and register default hotkeys
        /// </summary>
        /// <param name="_this">Mainwindown object</param>
        public static void initHotkeys(Window _this)
        {
            var helper = new WindowInteropHelper(_this);
            _source = HwndSource.FromHwnd(helper.EnsureHandle());
            _source.AddHook(HwndHook);
        }

        /// <summary>
        /// unregistered all hotkeys
        /// </summary>
        /// <param name="_this">MainWindow object</param>
        public static void terminateHotkeys(Window _this)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey(_this);
        }

        /// <summary>
        /// register a new hotkey
        /// </summary>
        /// <param name="_this">MainWindow object</param>
        /// <param name="MOD_CTRL">modifier</param>
        /// <param name="VK_CODE">virtual keycode</param>
        public static void RegisterHotKey(Window _this, uint MOD_CTRL, uint VK_CODE)
        {
            var helper = new WindowInteropHelper(_this);

            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_CODE))
            {
            }
        }

        private static void UnregisterHotKey(Window _this)
        {
            var helper = new WindowInteropHelper(_this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed(lParam);
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private static void OnHotKeyPressed(IntPtr lParam)
        {
            RegisteredHotkeyPressed(lParam);
        }

        /// <summary>
        /// returns tuple of pressed modifiers
        /// </summary>
        /// <param name="modCode">modCode for combined modifiers</param>
        /// <returns>isShift, isCtrl, isWin, isAlt</returns>
        public static Tuple<bool, bool, bool, bool> getBoolFromCode(uint modCode)
        {
            bool shift = false, alt = false, win = false, ctrl = false;

            if ((modCode & 0x1) == 1)
                alt = true;
            if ((modCode & 0x2) == 2)
                ctrl = true;
            if ((modCode & 0x4) == 4)
                shift = true;
            if ((modCode & 0x8) == 8)
                win = true;

            return Tuple.Create(shift, ctrl, win, alt);
        }

        /// <summary>
        /// returns modifier code of requested modifiers
        /// </summary>
        /// <param name="isShift">isShift attribute</param>
        /// <param name="isCtrl">isCtrl attribute</param>
        /// <param name="isWin">isWin attribute</param>
        /// <param name="isAlt">isAlt attribute</param>
        /// <returns>modCode for combined modifiers</returns>
        public static uint getCodeFromBool(bool? isShift, bool? isCtrl, bool? isWin, bool? isAlt)
        {
            uint result = 0;

            if (isAlt == true)
            {
                result = result | 0x1;
            }
            if (isCtrl == true)
            {
                result = result | 0x2;
            }
            if (isShift == true)
            {
                result = result | 0x4;
            }
            if (isWin == true)
            {
                result = result | 0x8;
            }

            return result;
        }
    }
}