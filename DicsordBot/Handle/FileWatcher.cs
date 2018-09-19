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
        /// <summary>
        /// checks wether the filetype is on the whitelist
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns>true, if file is allowed and supported</returns>
        public static bool checkForValidFile(string path)
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

        /// <summary>
        /// get all important information from a given file
        /// </summary>
        /// <param name="FullPath">Path to file</param>
        /// <returns>Data.FileData object, describing the file</returns>
        public static Data.FileData getAllFileInfo(string FullPath)
        {
            return getAllFileInfo(FullPath, Path.GetFileName(FullPath));
        }

        private static Data.FileData getAllFileInfo(string FullPath, string Name)
        {
            Data.FileData file = new Data.FileData();

            //name is set based on file name
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

            //only set if genres are existing
            var genres = f.Tag.Genres;
            if (genres.Length > 0)
                file.Genre = genres[0];

            var album = f.Tag.Album;
            if (album != null)
                file.Album = album;

            var length = f.Properties.Duration;
            if (length != null)
                file.Length = length;

            f.Dispose();
            return file;
        }

        /// <summary>
        /// updates list of files, deletes old list, runs in background thread
        /// </summary>
        /// <param name="sources">all directories to search</param>
        /// <param name="allowDuplicates">if true, doesn't test if file is already in list</param>
        public static void indexCleanFiles(ObservableCollection<string> sources, bool allowDuplicates = false)
        {
            Handle.Data.Files.Clear();
            indexFiles(sources, allowDuplicates);
        }

        /// <summary>
        /// updates list of files, keeps old list, runs in background thread
        /// </summary>
        /// <param name="sources">all directories to search</param>
        /// <param name="allowDuplicatos">if true, doesn't test if file is already in list</param>
        public static void indexFiles(ObservableCollection<string> sources, bool allowDuplicatos = false)
        {
            Thread worker = new Thread(() => indexFilesThread(sources, allowDuplicatos));

            worker.IsBackground = true;
            worker.Start();
        }

        private static void indexFilesThread(ObservableCollection<string> sources, bool allowDuplicates = false)
        {
            foreach (var dir in sources)
            {
                indexFolder(dir, allowDuplicates);
            }
            //sort by name
            Handle.Data.Files = new ObservableCollection<Data.FileData>(Handle.Data.Files.OrderBy(o => o.Name));
            Console.WriteLine("Finished indexing files");
        }

        /// <summary>
        /// recursive function for searching indexing a folder
        /// </summary>
        /// <param name="path">path to directory</param>
        /// <param name="allowDuplicates">if true, doesn't test if file is already in list</param>
        private static void indexFolder(string path, bool allowDuplicates)
        {
            string[] files = Directory.GetFiles(path);
            string[] subDirs = Directory.GetDirectories(path);

            foreach (var singleFile in files)
            {
                indexFile(singleFile, allowDuplicates);
            }

            foreach (var singleDir in subDirs)
            {
                indexFolder(singleDir, allowDuplicates);
            }
        }

        /// <summary>
        /// add file to list, if it's a supported format
        /// </summary>
        /// <param name="path">path to file</param>
        /// <param name="allowDuplicates">if true, doesn't test if file is already in list</param>
        private static void indexFile(string path, bool allowDuplicates)
        {
            if (!allowDuplicates)
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