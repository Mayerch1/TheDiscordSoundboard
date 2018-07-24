using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public partial class ButtonUI : INotifyPropertyChanged
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

        private void btn_FileChooser_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int index = (int)btn.Tag;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "mp3/wav files (*.mp3/*.wav)|*.mp3;*.wav|mp3 files (*.mp3)|*.mp3|wav files (*.wav)|*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
                //TODO: refresh fields
                Handle.Data.Persistent.BtnList[index].File = openFileDialog.FileName;
                if (Handle.Data.Persistent.BtnList[index].Name == null)
                {
                    Handle.Data.Persistent.BtnList[index].Name = evaluateName(openFileDialog.FileName);
                }
            }
        }

        //return only the file name from a Path to a file
        private string evaluateName(string filePath)
        {
            //TODO: refresh
            var fileType = System.IO.Path.GetFileName(filePath);
            var fileName = fileType.Substring(0, fileType.LastIndexOf('.'));

            return fileName;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }
    }
}