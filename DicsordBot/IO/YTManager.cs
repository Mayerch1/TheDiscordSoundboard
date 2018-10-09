using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace DicsordBot.IO
{
    /// <summary>
    /// Manages operations on youtube videos
    /// </summary>
    public static class YTManager
    {
        private const string imageUrl = "https://img.youtube.com/vi";

        private const string thumbnailQuality = "/sddefault.jpg";

        /// <summary>
        /// deletes all cached videos
        /// </summary>
        public static void clearVideoCache(string whiteList = "")
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.videoCacheFolder;

            if (Directory.Exists(folder))
            {
                foreach (var file in Directory.GetFiles(folder))
                {
                    if (file != whiteList)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        { continue; }
                    }
                }
            }
        }

        /// <summary>
        /// returns id of youtube video from given url
        /// </summary>
        /// <param name="url">full url to youtube video</param>
        /// <returns></returns>
        public static string getIdFromUrl(string url)
        {
            return url.Substring(url.LastIndexOf('=') + 1);
        }

        /// <summary>
        /// get url to Thumbnail from a given video url
        /// </summary>
        /// <param name="url">Url to yt video</param>
        /// <returns></returns>
        public static string getUrlToThumbnail(string url)
        {
            return imageUrl + getUrlToThumbnail(getIdFromUrl(url)) + thumbnailQuality;
        }

        /// <summary>
        /// download video from ulr
        /// </summary>
        /// <param name="url">url to yt video</param>
        /// <returns></returns>
        public static async Task<Video> getVideoAsync(string url)
        {
            //download video
            YouTube yt = YouTube.Default;
            Video vid;
            try
            {
                vid = await yt.GetVideoAsync(url);
            }
            catch (Exception ex)
            {
                UI.UnhandledException.initWindow(ex, "Error in downloading Video");
                Console.WriteLine("Exception.!?<");

                return null;
            }
            return vid;
        }

        /// <summary>
        /// saves a video into cache folder, this may block ui thread
        /// </summary>
        /// <param name="vid">video to save</param>
        private static string cacheVideo(Video vid)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.videoCacheFolder;
            //save video into cache folder
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var location = folder + @"\" + vid.FullName;
            try
            {
                //make async
                File.WriteAllBytes(location, vid.GetBytes());
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error on saving file");
                return null;
            }

            return location;
        }

        /// <summary>
        /// calls the blocking cacheVideo, saves result in BackgoundWorker e.Result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void worker_cacheVideo(object sender, DoWorkEventArgs e)
        {
            Video vid = e.Argument as Video;
            if (vid != null)
                e.Result = cacheVideo(vid);
            else
                e.Result = null;
        }

        /// <summary>
        /// can be called when worker is finished, gets result from ...EventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            string path = e.Result as string;
        }
    }
}