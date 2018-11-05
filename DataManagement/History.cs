namespace DataManagement
{
    /// <summary>
    /// extends Playlist to manage History with maximum amount of titles
    /// </summary>
    public class History : Playlist
    {
        private const int maxHistoryLen = 50;

        /// <summary>
        /// Default constructor of history
        /// </summary>
        public History() : base("History", "/res/empty.png") { }

        /// <summary>
        /// add Title to history, removes any title above max Length of history
        /// </summary>
        /// <param name="title"></param>
        public void addTitle(FileData title)
        {
            base.Tracks.Insert(0, title);
            while (base.Tracks.Count > maxHistoryLen)
            {
                base.Tracks.RemoveAt(base.Tracks.Count - 1);
            }
        }
    }
}