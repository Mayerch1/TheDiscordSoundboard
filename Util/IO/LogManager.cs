using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Util.IO
{
    public static class LogManager
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                              @"\" + DataManagement.PersistentData.defaultFolderName + @"\logs" + @"\";

        private static string name;

        private static string file = path + name;


        /// <summary>
        /// Initializes the logFile for this session
        /// </summary>
        public static void InitLog()
        {
            path += DateTime.Today.ToString("yyyy") + @"\" + DateTime.Today.ToString("MMM") + @"\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            //get timeString and first file
            string timeStr = DateTime.Today.ToString("yyyy-M-d");
            name = timeStr + ".log.txt";

            file = path + name;

            int ctr = 0;

            while (File.Exists(file))
            {
                ctr++;
                name = timeStr + "-" + ctr + ".log.txt";
                file = path + name;
            }
        }

        public static void OpenLog()
        {
            System.Diagnostics.Process.Start(file);
        }

        /// <summary>
        /// writes a specific Exception into the log
        /// </summary>
        /// <param name="ex">Exception to be logged</param>
        public static async void LogException(Exception ex, string occurence, string info, bool vital = false)
        {
            string date = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
            occurence = "[" + occurence + "]";

            try
            {
                using (StreamWriter sw = File.AppendText(file))
                {
                    await sw.WriteLineAsync(date + " " + occurence + " " + info + " | " + ex.Message);
                    if (vital)
                        await sw.WriteLineAsync(ExceptionExtractor(ex, occurence, info));
                }
            }
            catch
            { }
        }


        private static string ExceptionExtractor(Exception Ex, string Location, string Info)
        {
            string border =
                "======================================================================\r\n"
                + "======================================================================\r\n";

            string message = "A fatal error occurred (" + Info + ")\r\n";

            string traceMsg = "---------Detailed Information--------\r\n";


            string trace = "";

            if (Ex != null)
            {
                trace += "Exception Type: ";
                trace += Ex.GetType().ToString() + "\r\n";

                var st = new StackTrace(Ex, true);
                var frame = st.GetFrame(0);


                trace += "Method: " + frame.GetMethod().ToString()
                                    + "\r\n";

                trace += "Stack: " + frame.GetMethod().DeclaringType.ToString() + "\r\n";


                trace += "-----Complete Stacktrace----\r\n";


                foreach (var element in st.GetFrames())
                {
                    trace += element.GetMethod().DeclaringType.ToString() + "\r\n";
                }
            }

            return border + message + traceMsg + trace + border;
        }       
    }
}