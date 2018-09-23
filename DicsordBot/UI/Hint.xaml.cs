using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DicsordBot.UI
{
    /// <summary>
    /// Interaction logic for Hint.xaml
    /// </summary>
    public partial class Hint : UserControl, INotifyPropertyChanged
    {
#pragma warning disable CS1591
        private string errorMsg, solution;
        private bool ignoreWarning = false;

        public string ErrorMsg { get { return errorMsg; } set { errorMsg = value; OnPropertyChanged("ErrorMsg"); } }
        public string Solution { get { return solution; } set { solution = value; OnPropertyChanged("Solution"); } }
        public bool IgnoreWarning { get { return ignoreWarning; } set { ignoreWarning = value; OnPropertyChanged("IgnoreWarning"); } }

        public Hint(string msg, string solution)
        {
            InitializeComponent();

            ErrorMsg = msg;
            Solution = solution;

            ErrorBox.Text = ErrorMsg;
            SolutionBox.Text = Solution;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (box.IsChecked == true)
                IgnoreWarning = true;
            else
                IgnoreWarning = false;
        }

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(info));
            }
        }

#pragma warning restore CS1591
    }
}