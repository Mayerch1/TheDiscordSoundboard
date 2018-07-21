using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for UnhandledException.xaml
    /// </summary>
    public partial class UnhandledException : UserControl
    {
        private Exception Ex { get; set; }
        private string Info { get; set; }
        private int LineNumber { get; set; }
        private string FileName { get; set; }
        private string Method { get; set; }
        private string Class { get; set; }

        public UnhandledException(Exception _ex, string _Info = "")
        {
            InitializeComponent();
            Ex = _ex;
            Info = _Info;

            var st = new StackTrace(Ex, true);
            var frame = st.GetFrame(0);

            LineNumber = frame.GetFileLineNumber();
            FileName = frame.GetFileName();
            Method = frame.GetMethod().ToString();
            Class = frame.GetMethod().DeclaringType.ToString();
        }

        public static void initWindow(Exception _ex, string _Info = "")
        {
            Window window = new Window
            {
                Title = "Unhandled Exception caught",
                Content = new UnhandledException(_ex, _Info),
            };
        }
    }
}