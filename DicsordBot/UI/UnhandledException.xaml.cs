using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DiscordBot.UI
{
    /// <summary>
    /// Interaction logic for UnhandledException.xaml
    /// </summary>
    public partial class UnhandledException : UserControl, INotifyPropertyChanged
    {
#pragma warning disable CS1591

        private Exception ex;
        private string info;
        private int lineNumber;
        private int columnNumber;
        private string fileName;
        private string method;
        private string className;
        private string exString = "A null referenced expeption was caught";

        public Exception Ex { get { return ex; } set { ex = value; OnPropertyChanged("Ex"); } }
        public string Info { get { return info; } set { info = value; OnPropertyChanged("Info"); } }
        public int LineNumber { get { return lineNumber; } set { lineNumber = value; OnPropertyChanged("LineNumber"); } }
        public int ColumnNumber { get { return columnNumber; } set { columnNumber = value; OnPropertyChanged("ColumnNumber"); } }

        public string FileName { get { return fileName; } set { fileName = value; OnPropertyChanged("FileName"); } }
        public string Method { get { return method; } set { method = value; OnPropertyChanged("Method"); } }
        public string Class { get { return className; } set { className = value; OnPropertyChanged("Class"); } }

        public string ExString { get { return exString; } set { exString = value; OnPropertyChanged("ExString"); } }

        public List<MyStack> StackTrace { get; set; }
        public List<string> MethodTrace { get; set; }

        public UnhandledException(Exception _ex, string _Info = "")
        {
            InitializeComponent();
            StackTrace = new List<MyStack>();

            Ex = _ex;
            Info = _Info;

            if (Ex != null)
            {
                var st = new StackTrace(Ex, true);
                var frame = st.GetFrame(0);

                for (int i = 0; i < st.FrameCount; i++)
                {
                    StackTrace.Add(new MyStack(st.GetFrame(i).GetMethod().DeclaringType.ToString()));
                }

                LineNumber = frame.GetFileLineNumber();
                ColumnNumber = frame.GetFileColumnNumber();
                FileName = frame.GetFileName();
                Method = frame.GetMethod().ToString();
                Class = frame.GetMethod().DeclaringType.ToString();

                StackTraceTemplate.ItemsSource = StackTrace;

                ExString = "Occured when: " + Info + "\n";
                ExString += "Error: " + Ex.Message + "\n";

                ExString += "--Developer Information--\n";
                ExString += "Stack: " + Class + "\n";
                ExString += "File: " + FileName + "\n";
                ExString += "Method: " + Method + "\n";
                ExString += "Line: " + LineNumber + "\n";
                ExString += "Column: " + ColumnNumber + "\n";

                ExString += "--Complete Stacktrace--\n";
                foreach (var lvl in StackTrace)
                {
                    ExString += lvl.Stack + "\n";
                }
            }

            this.DataContext = this;
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Data.PersistentData.urlToGitRepo + "issues/new/choose");
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ExString);
        }

        public static void initWindow(Exception _ex, string _Info = "")
        {
            Window window = new Window
            {
                Title = "Some sort of error occured",
                Content = new UnhandledException(_ex, _Info),
            };
            window.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class MyStack
    {
        public MyStack(string n)
        {
            Stack = n;
        }

        public string Stack { get; set; }
    }

#pragma warning restore CS1591
}