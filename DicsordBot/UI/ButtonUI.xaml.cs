using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DicsordBot.UI
{
    /// <summary>
    /// Interaction logic for ButtonUI.xaml
    /// </summary>
    public partial class ButtonUI : INotifyPropertyChanged
    {
#pragma warning disable CS1591

        public delegate void InstantButtonClickedHandler(int btnListIndex);

        public InstantButtonClickedHandler InstantButtonClicked;

        public delegate void ToggleHotkeyHandler(bool isEnabled);

        public ToggleHotkeyHandler ToggleHotkey;

        public ButtonUI()
        {
            InitializeComponent();

            Handle.Data.resizeBtnList();

            this.DataContext = Handle.Data.Persistent;
            btnControl.ItemsSource = Handle.Data.Persistent.BtnList;
        }

        private void btn_Instant_Click(object sender, RoutedEventArgs e)
        {
            //event is handled in MainWindow

            Button btn = (Button)sender;

            int index = (int)btn.Tag;

            InstantButtonClicked(index);
        }

        private void btn_FileChooser_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int index = (int)btn.Tag;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            string allFormats = "*" + Handle.Data.Persistent.supportedFormats.Aggregate((i, j) => i + ";" + "*" + j);
            string allFormatString = "all supported types |" + allFormats;

            openFileDialog.Filter = allFormatString +
                                    "|mp3/wav files (*.mp3/*.wav)|*.mp3;*.wav" +
                                    "|all files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true && openFileDialog.CheckFileExists)
            {
                Handle.Data.Persistent.BtnList[index].Name = evaluateName(openFileDialog.FileName);
                Handle.Data.Persistent.BtnList[index].File = openFileDialog.FileName;

                var parent = (StackPanel)btn.Parent;

                // changeBackFields(parent, index);
                // changeFrontFields(parent, index);
            }
        }

        private void btn_HotkeyEditor_Click(object sender, RoutedEventArgs e)
        {
            int tag = (int)((FrameworkElement)sender).Tag;

            var dialog = new ButtonHotkeyWindow(tag, Application.Current.MainWindow);
            dialog.Owner = Application.Current.MainWindow;
            dialog.Topmost = true;

            //disable hotkeys, while editing them
            ToggleHotkey(false);
            //trigger blur effect
            BlurEffectManager.ToggleBlurEffect(true);

            //remove effects on close of dialog
            dialog.Closing += delegate (object dSender, CancelEventArgs dE)
            {
                BlurEffectManager.ToggleBlurEffect(false);
                ToggleHotkey(true);
            };

            dialog.Show();
        }

        private void changeBackFields(StackPanel parent, int index)
        {
            var sibblings = parent.Children;

            foreach (var element in sibblings)
            {
                if (element is System.Windows.Controls.TextBox)
                {
                    var txtBox = (TextBox)element;
                    if (txtBox.Name == "NameBox")
                    {
                        txtBox.Text = Handle.Data.Persistent.BtnList[index].Name;
                    }
                    else if (txtBox.Name == "FileBox")
                    {
                        txtBox.Text = Handle.Data.Persistent.BtnList[index].File;
                    }
                }
            }
        }

        private void changeFrontFields(StackPanel parent, int index)
        {
            var grid = (Grid)this.Content;

            var gridChild = (UIElementCollection)grid.Children;
            var scroller = (ScrollViewer)gridChild[0];

            var control = (ItemsControl)scroller.Content;
            var cards = control.Items;

            var targetCard = cards[index];

            var grandParents = (Grid)parent.Parent;
        }

        //return only the file name from a Path to a file
        private string evaluateName(string filePath)
        {
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

        private void btn_Return_Click(object sender, RoutedEventArgs e)
        {
            Handle.Data.resizeBtnList();
        }

#pragma warning restore CS1591
    }
}