using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    /// <summary>
    /// static class for monitoring files on disk
    /// </summary>
    public class FileWatcher
    {
        private static bool checkForValidFile(string path)
        {
            string format = path.Substring(path.LastIndexOf('.') + 1);

            foreach (var testFormat in Handle.Data.Persistent.supportedFormats)
                if (testFormat == format) return true;

            return false;
        }

        private static Data.FileData getAllFileInfo(FileSystemEventArgs e)
        {
            Data.FileData file = new Data.FileData();

            file.Name = e.Name.Remove(e.Name.LastIndexOf('.'));
            file.Path = e.FullPath;

            TagLib.File f;
            try
            {
                f = TagLib.File.Create(e.FullPath);
            }
            catch { return file; }

            //only set, when TagLib could load file
            var performers = f.Tag.Performers;
            if (performers.Length > 0)
                file.Author = performers[0];

            file.Length = f.Properties.Duration;

            f.Dispose();
            return file;
        }

        /// <summary>
        /// inits one watcher for each path
        /// </summary>
        /// <param name="sources">collection of paths to directories to watch</param>
        public static void StartMonitor(ObservableCollection<string> sources)
        {
            foreach (var path in sources)
                initWatcher(path);
        }

        /// <summary>
        /// init watcher for the specific path
        /// </summary>
        /// <param name="source">path to monitored directory</param>
        public static void initWatcher(string source)
        {
            FileSystemWatcher fsW = new FileSystemWatcher();
            fsW.Path = source;
            fsW.IncludeSubdirectories = true;

            fsW.Changed += FileSystemWatcher_Changed;
            fsW.Created += FileSystemWatcher_Created;
            fsW.Renamed += FileSystemWatcher_Renamed;
            fsW.Deleted += FileSystemWatcher_Deleted;

            fsW.EnableRaisingEvents = true;
        }

        /// <summary>
        /// when new file is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (checkForValidFile(e.FullPath))
            {
                Data.FileData nFile = getAllFileInfo(e);

                Handle.Data.Files.Add(nFile);
            }
        }

        /// <summary>
        /// when file was renamed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            string oldPath = ((RenamedEventArgs)e).OldFullPath;

            for (int i = 0; i < Handle.Data.Files.Count; i++)
            {
                if (Handle.Data.Files[i].Path == oldPath)
                {
                    Handle.Data.Files[i] = getAllFileInfo(e);
                    break;
                }
            }
        }

        /// <summary>
        /// when file was changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            for (int i = 0; i < Handle.Data.Files.Count; i++)
            {
                if (Handle.Data.Files[i].Path == e.FullPath)
                {
                    Handle.Data.Files[i] = getAllFileInfo(e);
                    break;
                }
            }
        }

        /// <summary>
        /// when file was deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            for (int i = 0; i < Handle.Data.Files.Count; i++)
            {
                if (Handle.Data.Files[i].Path == e.FullPath)
                {
                    Handle.Data.Files.RemoveAt(i);
                    break;
                }
            }
            Console.WriteLine("Test");
        }
    }
}