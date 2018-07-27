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

        private List<Data.ButtonData> BtnList
        {
            get { return Handle.Data.Persistent.BtnList; }
            set { Handle.Data.Persistent.BtnList = value; OnPropertyChanged("BtnList"); }
        }

        public ButtonUI()
        {
            InitializeComponent();

            Handle.Data.resizeBtnList();

            btnControl.ItemsSource = BtnList;

            this.DataContext = this;
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
            openFileDialog.Filter = "all supported types (*.mp3/*.wav/*.asf/*.wma/*.wmv/*.sami/*.smi/*.3g2/*.3gp/*.3gp2/*.3gpp/*.aac/*.adts/*.m4a/*.m4v/*.mov/*.mp4)|*.mp3;*.wav;*.asf;*.wma;*.wmv;*.sami;*.smi;*.3g2;*.3gp;*.3gp2;*.3gpp;*.aac;*.adts;*.m4a;*.m4v;*.mov;*.mp4" +
                                    "|mp3/wav files (*.mp3/*.wav)|*.mp3;*.wav" +
                                     "|all files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true && openFileDialog.CheckFileExists)
            {
                BtnList[index].Name = evaluateName(openFileDialog.FileName);
                BtnList[index].File = openFileDialog.FileName;
            }
            //TODO: refresh button on fileChooser click
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
    }
}