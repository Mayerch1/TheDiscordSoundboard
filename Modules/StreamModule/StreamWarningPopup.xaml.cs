using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StreamModule
{
#pragma warning disable CS1591
    /// <summary>
    /// Interaction logic for StreamWarningPopup.xaml
    /// </summary>
    public partial class StreamWarningPopup : Popup
    {
        public bool eula = false;
        public StreamWarningPopup(Window window)
        {
            this.PlacementTarget = window;
            this.Placement = PlacementMode.Center;
            InitializeComponent();
            this.StaysOpen = false;
        }

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            eula = true;
            IsOpen = false;
        }

        private void btn_Decline_Click(object sender, RoutedEventArgs e)
        {
            eula = false;
            IsOpen = false;
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
