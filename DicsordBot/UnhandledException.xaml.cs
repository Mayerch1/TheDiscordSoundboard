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
        public Exception Ex { get; set; }
        public string Info { get; set; }
        public int Line { get; set; }
        public string Document { get; set; }

        public UnhandledException(Exception _ex, string _Info = "")
        {
            InitializeComponent();
        }

        public static void initWindow(Exception _ex, string _Info = "")
        {
            var st = new StackTrace(_ex, true);

            var query = st.GetFrames()
                        .Select(frame => new
                        {
                            FileName = frame.GetFileName(),
                            LineNumber = frame.GetFileLineNumber(),
                            ColumnNumber = frame.GetFileColumnNumber(),
                            Method = frame.GetMethod(),
                            Class = frame.GetMethod().DeclaringType,
                        });

            //Window window = new Window
            //{
            //    Title = "Unhandled Exception caught",
            //    Content = new UnhandledException(_ex, query),
            //};
        }
    }
}