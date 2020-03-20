using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Util.IO;

using VideoLibrary;


namespace StreamModule
{
    /// <summary>
    /// Downloads video, chooses correct download engine based on link
    /// </summary>
    public static class DownloadManager
    {
        /// <summary>
        /// Result of a Caching progress
        /// </summary>
        public struct CacheResult
        {
            /// <summary>
            /// uri to video
            /// </summary>
            public string uri;

            /// <summary>
            /// location on disk
            /// </summary>
            public string location;
        }

        private static bool mutex = false;

        private static string url = String.Empty;

        private static string dlUri = null;
        private static Video ytVid = null;


        /// <summary>
        /// Prepares the caching process by gathering all information w/ low bandwidth use
        /// </summary>
        /// <param name="url">url to video</param>
        /// <param name="title">title of the video, for hashing the filename</param>
        /// <param name="orideMutex">ignores the mutex locking the request</param>
        public static async Task prepareCacheVideoAsync(string url, string title, bool orideMutex = false)
        {
            while (mutex && !orideMutex)
                await Task.Delay(25);

            if (!mutex || orideMutex)
            {
                if (!orideMutex)
                    mutex = true;
                DownloadManager.url = url;

                ytVid = null;
                dlUri = null;

                if (url.Contains("youtube.com") || url.Contains("youtu.be"))
                {
                    ytVid = await prepareYTCacheAsync(url);
                }
                else
                    dlUri = url;

                if (!orideMutex)
                    mutex = false;
            }
        }

        /// <summary>
        /// Caches the video to file, or gets the stream
        /// </summary>
        /// <param name="url">url to video</param>
        /// <param name="title">title of video</param>
        /// <returns></returns>
        public static async Task<CacheResult> cacheVideoAsync(string url, string title)
        {
            var result = new CacheResult();

            //wait until mutex is free
            while (mutex)
                await Task.Delay(10);

            if (!mutex)
            {
                mutex = true;

                //make preparation if not done before, or if different url
                if (DownloadManager.url != url || (ytVid == null && dlUri == null))
                {
                    await prepareCacheVideoAsync(url, title, true);
                }

                if (ytVid != null)
                {
                    result.uri = await ytVid.GetUriAsync();
                    result.location = "";
                }
                else if (dlUri != null)
                {
                    result.uri = await Task.Run(getMiscUri);
                    result.location = "";
                }

                mutex = false;
            }

            return result;
        }


        private static async Task<Video> prepareYTCacheAsync(string url)
        {
            //download video
            YouTube yt = YouTube.Default;
            
            Video mpAudio;

            VideoClient videoClient = new VideoClient();
            try
            {
                //get video file
                var videos = await YouTube.Default.GetAllVideosAsync(url);
             
                //get audios, only aac
                var audios = videos
                    .Where(v => v.AudioFormat != AudioFormat.Unknown && v.AudioFormat != AudioFormat.Vorbis).ToList();

                //save audio into Video, only with audio
                mpAudio = audios.FirstOrDefault(x => x.AudioBitrate > 0);
            }
            catch (Exception ex)
            {
                SnackbarManager.SnackbarMessage("Could not load video, unknown reason", SnackbarManager.SnackbarAction.Log);

                Util.IO.LogManager.LogException(ex, "StreamModule/YTManager",
                    "Error in requesting Video/Audio information, please report on GitHub");
                return null;
            }

            return mpAudio;
        }


#pragma warning disable CS1998
        private static async Task<string> getMiscUri()
        {
            string arg = "--get-url " + dlUri;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = DataManagement.PersistentData.youtubeDLPath,
                    Arguments = arg,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                if (line != null)
                {
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
                        return line;
                }
            }

            return "";
        }
#pragma warning restore CS1998
    }
}