using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
    /// <summary>
    /// Interaction logic for ButtonUI.xaml
    /// </summary>
    public partial class ButtonUI
    {
        public delegate void InstantButtonClickedHandler(int btnListIndex);

        public InstantButtonClickedHandler InstantButtonClicked;

        public ButtonUI()
        {
            InitializeComponent();

            Handle.Data.resizeBtnList();

            btnControl.ItemsSource = Handle.Data.Persistent.BtnList;

            this.DataContext = this;
        }

        private void btn_Instant_Click(object sender, RoutedEventArgs e)
        {
            //event is handled in MainWindow          

            Button btn = (Button)sender;

            

            int index = (int)btn.Tag;

            InstantButtonClicked(index);
        }

        private void btn_Instant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ////open settings for that Button
                //Button btn = (Button)sender;
                //int index = (int)btn.Tag;

                //openButtonSettings(index);
            }
        }

        private void openButtonSettings(int index)
        {
            Window window = new Window
            {
                Title = "Settings",
                Content = new ButtonSettingUI(index),
            };

            window.ShowDialog();
        }

    }
}