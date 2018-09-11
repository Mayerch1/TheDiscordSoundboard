using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DicsordBot
{
    /// <summary>
    /// static class for monitoring files on disk
    /// </summary>
    public static class FileWatcher
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
            return getAllFileInfo(e.FullPath, e.Name);
        }

        private static Data.FileData getAllFileInfo(string FullPath)
        {
            return getAllFileInfo(FullPath, Path.GetFileName(FullPath));
        }

        private static Data.FileData getAllFileInfo(string FullPath, string Name)
        {
            Data.FileData file = new Data.FileData();

            file.Name = Name.Remove(Name.LastIndexOf('.'));
            file.Path = FullPath;

            TagLib.File f;
            try
            {
                f = TagLib.File.Create(FullPath);
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
        /// updates list of files, keeps old list, runs in background thread
        /// </summary>
        /// <param name="sources">all directories to search</param>
        /// <param name="ignoreDuplicates">if true, doesn't test if file is already in list</param>
        public static void indexFiles(ObservableCollection<string> sources, bool ignoreDuplicates = false)
        {
            Thread worker = new Thread(() => indexFilesThread(sources, ignoreDuplicates));

            worker.IsBackground = true;
            worker.Start();
        }

        private static void indexFilesThread(ObservableCollection<string> sources, bool ignoreDuplicates = false)
        {
            foreach (var dir in sources)
            {
                indexFolder(dir, ignoreDuplicates);
            }
        }

        /// <summary>
        /// recursive function for searching indexing a folder
        /// </summary>
        /// <param name="path">path to directory</param>
        /// <param name="ignoreDuplicates">if true, doesn't test if file is already in list</param>
        private static void indexFolder(string path, bool ignoreDuplicates)
        {
            string[] files = Directory.GetFiles(path);
            string[] subDirs = Directory.GetDirectories(path);

            foreach (var singleFile in files)
            {
                indexFile(singleFile, ignoreDuplicates);
            }

            foreach (var singleDir in subDirs)
            {
                indexFolder(singleDir, ignoreDuplicates);
            }
        }

        /// <summary>
        /// add file to list, if it's a supported format
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="ignoreDuplicates">if true, doesn't test if file is already in list</param>
        private static void indexFile(string path, bool ignoreDuplicates)
        {
            if (!ignoreDuplicates)
            {
                //check for existing files, to avoid duplicates
                foreach (var singleFile in Handle.Data.Files)
                {
                    if (singleFile.Path == path) return;
                }
            }

            if (checkForValidFile(path))
            {
                Handle.Data.Files.Add(getAllFileInfo(path));
            }
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
        /// if a new sources was added, this adds a fileWatcher for that directory. <see cref="indexFiles(ObservableCollection{string}, bool)"/> should also be called
        /// </summary>
        ///
        /// <param name="source">path to directory</param>
        public static void addWatcher(string source)
        {
            initWatcher(source);
        }

        private static void initWatcher(string source)
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

        #region events

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (checkForValidFile(e.FullPath))
            {
                Data.FileData nFile = getAllFileInfo(e);

                Handle.Data.Files.Add(nFile);
            }
        }

        private static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
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

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
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

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            for (int i = 0; i < Handle.Data.Files.Count; i++)
            {
                if (Handle.Data.Files[i].Path == e.FullPath)
                {
                    Handle.Data.Files.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion events
    }
}