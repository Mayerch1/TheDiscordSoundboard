using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace DeviceStreamModule
{
    /// <summary>
    /// Interaction logic for DeviceMode.xaml
    /// </summary>
    public partial class DeviceMode : UserControl, INotifyPropertyChanged
    {
        // string frames for tooltip
        private const string _tooltipFrameA = "Not supported! Audio devices should have ";
        private const string _tooltipFrameB = " There might be problems with the audio streams";
        
        private const int _channelCount = 2;
        private const int _sampleRate = 48000;
        private const int _bitDepth = 16;

        //tooltip for indicators of incompatible settings. needs to be public for accessibility from xaml
        public static readonly string _channelCountTooltip = _tooltipFrameA + _channelCount + " Channels." +  _tooltipFrameB;
        public static readonly string _sampleRateTooltip = _tooltipFrameA + _sampleRate +"Hz."+ _tooltipFrameB;

        public delegate void DeviceStartStreamHandler(string name, string id);

        public DeviceStartStreamHandler DeviceStartStream;

        public delegate void DeviceStopStreamHandler(string id);

        public DeviceStopStreamHandler DeviceStopStream;


        private string selectedDevice = null;
        private string selectedDeviceName = null;
        private int channelCount = 0;
        private int sampleRate = 0;
        private int bitDepth = 0;       


        public int ChannelCount
        {
            get => channelCount;
            set
            {
                channelCount = value;
                OnPropertyChanged("ChannelCount");
            }
        }

        public int SampleRate
        {
            get => sampleRate;
            set
            {
                sampleRate = value;
                OnPropertyChanged("SampleRate");
            }
        }

        public int BitDepth
        {
            get => bitDepth;
            set
            {
                bitDepth = value;
                OnPropertyChanged("BitDepth");
            }
        }




        public DeviceMode()
        {
            InitializeComponent();

           
            this.DataContext = this;

            findDevices();
        }


        private void findDevices()
        {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();

            //loop through all available and active devices
            foreach (MMDevice device in devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                //add the device to the dropdown list
                var waveFormat = device.AudioClient.MixFormat;

                var element = new BoxElement()
                {
                    Name = device.FriendlyName,
                    Id = device.ID,
                    sampleRate = waveFormat.SampleRate,
                    channelCount = waveFormat.Channels,
                    bitDepth = waveFormat.BitsPerSample,
                };


                ComboBox.Items.Add(element);   
                //Console.WriteLine(device.FriendlyName + "\t" + device.State);
            }

            //add a virtual earrape device
            Random r = new Random();
            ComboBox.Items.Add(new BoxElement("Death and Destruction", "-1", r.Next(100000), r.Next(6), r.Next(32)));
        }

       

      
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //start streaming the selected device
            if (selectedDevice != null)
                DeviceStartStream(selectedDeviceName, selectedDevice);
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is BoxElement device)
            {
                //set all properties for displaying the selected item
                selectedDevice = device.Id;
                selectedDeviceName = device.Name;
                ChannelCount = device.channelCount;
                SampleRate = device.sampleRate;
                BitDepth = device.bitDepth;

                
                ToggleInvalidSettings(SampleRate==_sampleRate, ChannelCount==_channelCount, BitDepth==_bitDepth);
            }
        }


        /// <summary>
        /// Toggle the warning for incompatible formats
        /// </summary>
        /// <param name="sampleValid">if SampleRate is compatible</param>
        /// <param name="channelValid">if ChannelCount is compatible</param>
        /// <param name="bitValid">if BitDepth is compatible</param>
        private void ToggleInvalidSettings(bool sampleValid, bool channelValid, bool bitValid)
        {
            Icon_Sample_Valid.Visibility = !sampleValid ? Visibility.Visible : Visibility.Hidden;
            Icon_Channel_Valid.Visibility = !channelValid ? Visibility.Visible : Visibility.Hidden;
            //Icon_Bit_Valid.Visibility = !bitValid ? Visibility.Visible : Visibility.Hidden;
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private struct BoxElement
        {
            public BoxElement(string name, string id, int sRate=0, int chCnt = 0, int bitDpth=0)
            {
                Id = id;
                Name = name;
                sampleRate = sRate;
                bitDepth = bitDpth;
                channelCount = chCnt;
            }

            public override string ToString()
            {
                return Name;
            }

            public string Id;
            public string Name;
            public int sampleRate;
            public int bitDepth;
            public int channelCount;

        }

       
    }
}