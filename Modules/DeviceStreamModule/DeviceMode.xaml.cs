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
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace DeviceStreamModule
{
    /// <summary>
    /// Interaction logic for DeviceMode.xaml
    /// </summary>
    public partial class DeviceMode : UserControl, INotifyPropertyChanged
    {
        //private WasapiCapture _capture = null;


        public delegate void DeviceStartStreamHandler(string name, string id);

        public DeviceStartStreamHandler DeviceStartStream;

        public delegate void DeviceStopStreamHandler(string id);

        public DeviceStopStreamHandler DeviceStopStream;


        private string selectedDevice = null;
        private string selectedDeviceName = null;


        public DeviceMode()
        {
            InitializeComponent();

            this.DataContext = this;

            findDevices();
        }


        private void findDevices()
        {
            MMDeviceEnumerator devices = new MMDeviceEnumerator();

            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            foreach (MMDevice device in devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                ComboBox.Items.Add(new BoxElement(device.FriendlyName, device.ID));


                //Console.WriteLine(device.FriendlyName + "\t" + device.State);
            }

            //add a virtual earrape device
            ComboBox.Items.Add(new BoxElement("Death and Destruction", "-1"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

      
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedDevice != null)
                DeviceStartStream(selectedDeviceName, selectedDevice);
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is BoxElement device)
            {
                selectedDevice = device.Id;
                selectedDeviceName = device.Name;

                //_capture?.StopRecording();

                //_capture = new WasapiCapture(device);
                //_capture.DataAvailable += WaveInOnDataAvailable;
                //_capture.StartRecording();
            }
        }

        //takes to much cpu-time
        //private void WaveInOnDataAvailable(object sender, WaveInEventArgs e)
        //{
        //    float max = 0;
        //    var buffer = new WaveBuffer(e.Buffer);
        //    // interpret as 32 bit floating point audio
        //    for (int index = 0; index < e.BytesRecorded / 4; index++)
        //    {
        //        var sample = buffer.FloatBuffer[index];

        //        // absolute value 
        //        if (sample < 0) sample = -sample;
        //        // is this the max value?
        //        if (sample > max) max = sample;


        //    }
        //    ProgressVal = max * 100;
        //}


        private struct BoxElement
        {
            public BoxElement(string name, string id)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }

            public string Id;
            public string Name;

        }

       
    }
}