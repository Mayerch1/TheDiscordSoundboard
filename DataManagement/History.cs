using System.Linq;

namespace DataManagement
{
    /// <summary>
    /// extends Playlist to manage History with maximum amount of titles
    /// </summary>
    public class History : Playlist
    {

        /// <summary>
        /// Default constructor of history
        /// </summary>
        public History() : base("History", "/res/empty.png") { }

        /// <summary>
        /// add Title to history, removes any title above max Length of history
        /// </summary>
        /// <param name="title"></param>
        /// <param name="maxLen">max. number of files in list</param>
        public void addTitle(FileData title, int maxLen)
        {
            var oldFile = base.Tracks.FirstOrDefault(f => f.Path == title.Path);

            if (oldFile != null)
            {
                base.Tracks.Remove(oldFile);
            }

            base.Tracks.Insert(0, title);

            while (base.Tracks.Count > maxLen)
                base.Tracks.RemoveAt(base.Tracks.Count - 1);
            
        }
    }
}