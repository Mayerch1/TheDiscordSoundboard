using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement
{
    /// <summary>
    /// The queue of the bot, handles instantQueue as well as Playlist-queue
    /// </summary>
   public class BotQueue
    {
        private List<QueueItem> queue = new List<QueueItem>();
        private Playlist playList = null;
        private int trackId = 0;
        private bool isHistory;

        /// <summary>
        /// enqueue an item at the end of a list
        /// </summary>
        /// <param name="data"></param>
        /// <param name="disableHistory"></param>
        public void enqueue(BotData data, bool disableHistory)
        {
            queue.Add(new QueueItem(data, disableHistory));
        }

        /// <summary>
        /// enqueue an entire playlist
        /// </summary>
        /// <param name="list"></param>
        /// <param name="trackId"></param>
        /// <param name="history">marks history playlist</param>
        public void enqueuePlaylist(Playlist list, uint trackId, bool history)
        {
            playList = list;
            this.trackId = (int)trackId;
            isHistory = history;
        }

        /// <summary>
        /// remove the playlist, keep the single-item queue
        /// </summary>
        public void clearPlaylist()
        {
            playList = null;
            trackId = 0;
        }

        /// <summary>
        /// Clears all queue elements
        /// </summary>
        public void clearQueue()
        {
            clearPlaylist();
            queue.Clear();
        }

        
        /// <summary>
        /// Gets the next item in the queue
        /// </summary>
        /// <param name="isLoop">If LoopAll AND End of List -> returns index 0 of list</param>
        /// <returns>null if no item is queued</returns>
        public QueueItem? getNextItem(bool isLoop = false)
        {
            if (queue.Count > 0)
            {
                //get next queue item
                var data = queue[0];
                queue.RemoveAt(0);
                return data;
            }
            else if (playList != null)
            {
                //loop of playlist
                if (isLoop && trackId > playList.Tracks.Count)
                {
                    //skipTracks(uint num) depends on this func
                    trackId = 0;
                }

                //get next playlist item
                if (playList.Tracks.Count > trackId)
                {
                    //return the item and increase counter
                    var data = playList.Tracks[trackId++];
                    //if playlist is history, than it's not added to the history
                    return new QueueItem(new BotData(data.Name, data.Path, "",data.Author), isHistory);
                }
                //if playlist is empty, return null
            }
            return null;
        }

        /// <summary>
        /// skip tracks
        /// </summary>
        /// <param name="num"></param>
        public void skipTrack(uint num=1)
        {
            while (queue.Count > 0 && num>0)
            {
                queue.RemoveAt(0);
                --num;
            }
            

            //trackId outOfRange is treated in getNextItem
            trackId += (int)num;
            
        }

        /// <summary>
        /// backward-skip tracks, will not go below 0
        /// </summary>
        /// <param name="num"></param>
        public void skipBackTrack(uint num=1)
        {
            //queue cannot be back-skipped
            if (playList != null)
            {
                //if trackId < 0, Reset to 0
                trackId -= (int) num;
                if (trackId < 0)
                    trackId = 0;
            }
        }


        /// <summary>
        /// QueueItem
        /// </summary>
        public struct QueueItem
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">data element</param>
            /// <param name="disableHistory">if history is not recorded (e.g. video or history-file itself)</param>
            public QueueItem(BotData data, bool disableHistory)
            {
                botData = data;
                this.disableHistory = disableHistory;
            }
            /// <summary>
            /// Data prepared for bot
            /// </summary>
            public BotData botData;

            /// <summary>
            /// disableHistory field
            /// </summary>
            public bool disableHistory;
        }
    }
   
}
