using System;
using System.Windows.Navigation;

namespace SoundBoard.Misc
{
    /// <summary>
    /// manages playlist, skips tracks, etc
    /// </summary>
    public static class PlaylistManager
    {
        private static bool _isInitialized = false;

        private static bool _isLoop = false;


        private static int ListIndex { get; set; }

        private static int FileIndex { get; set; }


        //get current playlist or history out of list
        private static DataManagement.Playlist Playlist =>
            ListIndex >= 0 ? Handle.Data.Playlists[ListIndex] : Handle.Data.History;


        /// <summary>
        /// Initialize class with new list
        /// </summary>
        /// <param name="listIndex">index of list</param>
        /// <param name="fileIndex">first file to play</param>
        /// <return>first track</return>
        public static DataManagement.FileData InitList(int listIndex, int fileIndex)
        {
            ListIndex = listIndex;
            FileIndex = fileIndex;
            _isInitialized = true;

            if (FileIndex <= Playlist.Tracks.Count)
                return Playlist.Tracks[FileIndex];

            //if file not existing
            _isInitialized = false;
            return null;
        }

        /// <summary>
        /// changes the loop-state of the list
        /// </summary>
        /// <param name="isLoop">isLoop</param>
        public static void SetLoopState(bool isLoop)
        {
            _isLoop = isLoop;
        }

        /// <summary>
        /// get the next track in the list
        /// </summary>
        /// <returns>null on error or end of list</returns>
        public static DataManagement.FileData GetNextTrack()
        {
            if (!_isInitialized)
                return null;


            if (++FileIndex <= Playlist.Tracks.Count)
            {
                return Playlist.Tracks[FileIndex];
            }
            else
            {
                //restart playlist
                if (_isLoop)
                {
                    FileIndex = 0;
                    return Playlist.Tracks[0];
                }

                //end of list
                _isInitialized = false;
                _isLoop = false;
                return null;
            }
        }

        /// <summary>
        /// Skips negative 2 Tracks, so when skipping the current played after it, you will end at -1 relative to the current track
        /// </summary>
        public static void SkipBackwards()
        {
            if (_isInitialized)
            {
                if (FileIndex > 0)
                {
                    FileIndex -= 2;
                }
            }
        }
    }
}