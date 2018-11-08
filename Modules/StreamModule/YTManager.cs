using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Util.IO;
using VideoLibrary;
using YoutubeSearch;

namespace StreamModule
{
    /// <summary>
    /// Manages operations on youtube videos
    /// </summary>
    public static class YTManager
    {
        private const string imageUrl = "https://img.youtube.com/vi/";

        private const string thumbnailQuality = "/sddefault.jpg";


        /// <summary>
        /// deletes all cached videos
        /// </summary>
        public static void clearVideoCache(string whiteList = "")
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" +
                            DataManagement.PersistentData.defaultFolderName + @"\" + DataManagement.PersistentData.videoCacheFolder;

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
                        {
                            continue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// returns id of youtube video from given url
        /// </summary>
        /// <param name="url">full url to youtube video</param>
        /// <remarks>fallback for SearchQueryTaskAsync</remarks>
        /// <seealso cref="VideoSearch.SearchQueryTaskAsync"/>
        /// <returns>null if no url was entered</returns>
        public static string getIdFromUrl(string url)
        {
            if ((url.Contains("https://") || url.Contains("http://")) && url.Contains("="))
            {
                return url.Substring(url.LastIndexOf('=') + 1);
            }

            return null;         
        }

        /// <summary>
        /// get url to Thumbnail from a given video url
        /// </summary>
        /// <param name="url">Url to yt video</param>
        /// <returns>direct url to thumbnail</returns>
        public static string getUrlToThumbnail(string url)
        {
            return imageUrl + getIdFromUrl(url) + thumbnailQuality;
        }


        /// <summary>
        /// get the title from an url
        /// </summary>
        /// <param name="url">url to video</param>
        /// <remarks>fallback for SearchQueryTaskAsync</remarks>
        /// <seealso cref="VideoSearch.SearchQueryTaskAsync"/>
        /// <returns>task string representing the title</returns>
        public static async Task<string> GetTitleTask(string url)
        {
            var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
            return GetArgs(await new WebClient().DownloadStringTaskAsync(api), "title", '&');
        }

        private static string GetArgs(string args, string key, char query)
        {
            var iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1)
                    : string.Empty)[key];
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
            Video mpAudio;

            VideoClient videoClient = new VideoClient();

            try
            {              
                //-------------------------
                // getting the audio from yt is very slow,
                // it's faster to download the vid, even on 10Mbit/s
                //--------------------------

                //get video file
                var videos = await YouTube.Default.GetAllVideosAsync(url);

                //get audios, only aac
                var audios = videos.Where(v => v.AudioFormat != AudioFormat.Unknown && v.AudioFormat != AudioFormat.Vorbis).ToList();

                //save audio into Video, only with audio
                mpAudio = audios.FirstOrDefault(x => x.AudioBitrate > 0);
            }
            catch (Exception ex)
            {
                Util.IO.LogManager.LogException(ex, "StreamModule/YTManager", "Error in requesting Video information", true);
                return null;
            }

            return mpAudio;
        }

        /// <summary>
        /// return readable string from video object
        /// </summary>
        /// <param name="vid">video object</param>
        /// <returns>Readable stream</returns>
        public static async Task<Stream> getStreamAsync(Video vid)
        {
            using (VideoClient cl = new VideoClient())
            {
                try
                {
                    return await cl.StreamAsync(vid);
                }
                catch(Exception ex)
                {
                    Util.IO.LogManager.LogException(ex, "StreamModule/YTManager", "Could not retrieve stream object");
                    return null;
                }
            }
        }

        /// <summary>
        /// return readable string from url
        /// </summary>
        /// <param name="url">url to video</param>
        /// <returns>Readable stream</returns>
        public static async Task<Stream> getStreamAsync(string url)
        {
            return await getStreamAsync(await getVideoAsync(url));
        }

        /// <summary>
        /// saves a video into cache folder, this may block ui thread when disc is slow
        /// </summary>
        /// <param name="vid">video to save</param>
        public static async Task<string> cacheVideo(Video vid)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" +
                            DataManagement.PersistentData.defaultFolderName + @"\" + DataManagement.PersistentData.videoCacheFolder;
            //save video into cache folder
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //hash name so it cannot be searched easily (bc of copyright)
            var name = getHashSha256(vid.FullName) + vid.FileExtension;
            var location = folder + "\\" + name;

            try
            {
                //make async
                File.WriteAllBytes(location, await vid.GetBytesAsync());
            }
            catch (System.Net.Http.HttpRequestException)
            {
                SnackbarManager.SnackbarMessage("Could not decrypt video");
                return null;
            }
            catch (System.OutOfMemoryException)
            {
                SnackbarManager.SnackbarMessage("File too large");
                Console.WriteLine(@"File " + name + @" is to large to save");
            }
            catch (Exception ex)
            {
                SnackbarManager.SnackbarMessage("Failed to cache Video");
                Util.IO.LogManager.LogException(ex, "StreamModule/YTManager", "Failed to cache video");
                return null;
            }

            return location;
        }

        private static string getHashSha256(string title)
        {
            byte[] byteStr = Encoding.UTF8.GetBytes(title);

            byte[] hash = new SHA256Managed().ComputeHash(byteStr);

            return Convert.ToBase64String(hash).Replace('/', '_');
        }
    }
}