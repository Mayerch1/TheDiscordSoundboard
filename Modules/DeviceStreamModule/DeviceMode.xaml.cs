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

        public delegate void DeviceStartStreamHandler(string id);

        public DeviceStartStreamHandler DeviceStartStream;

        public delegate void DeviceStopStreamHandler(string id);

        public DeviceStopStreamHandler DeviceStopStream;


        private string selectedDevice = null;

        private List<MMDevice> deviceList = new List<MMDevice>();

        public List<MMDevice> DeviceList
        {
            get => deviceList;
            set
            {
                deviceList = value;
                OnPropertyChanged("DeviceList");
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

            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            //devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            foreach (MMDevice device in devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                DeviceList.Add(device);
                
                //Console.WriteLine(device.FriendlyName + "\t" + device.State);
            }       
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("");
            if (e.AddedItems[0] is MMDevice device)
            {
                selectedDevice = device.ID;
            
            }
        }
    }
}
