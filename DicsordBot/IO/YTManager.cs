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
using VideoLibrary;
using YoutubeSearch;

namespace DiscordBot.IO
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
                            Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.videoCacheFolder;

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
        /// <seealso cref="SearchQueryTaskAsync"/>
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
        /// <seealso cref="SearchQueryTaskAsync"/>
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
        /// get information for searchquery or id
        /// </summary>
        /// <param name="querystring">search string or video id</param>
        /// <param name="querypages">amount of pages to search</param>
        /// <returns></returns>
        public static async Task<List<VideoInformation>> SearchQueryTaskAsync(string querystring, int querypages)
        { 
            var items = new List<VideoInformation>();
            var webClient = new WebClient();
            for (int index1 = 1; index1 <= querypages; ++index1)
            {
                MatchCollection matchCollection = Regex.Matches(await webClient.DownloadStringTaskAsync("https://www.youtube.com/results?search_query=" + querystring + "&page=" + (object)index1), "<div class=\"yt-lockup-content\">.*?title=\"(?<NAME>.*?)\".*?</div></div></div></li>", RegexOptions.Singleline);
                for (int index2 = 0; index2 <= matchCollection.Count - 1; ++index2)
                {
                    var title = matchCollection[index2].Groups[1].Value;
                    var author = VideoItemHelper.cull(matchCollection[index2].Value, "/user/", "class").Replace('"', ' ').TrimStart().TrimEnd();
                    var description = VideoItemHelper.cull(matchCollection[index2].Value, "dir=\"ltr\" class=\"yt-uix-redirect-link\">", "</div>");
                    var duration = VideoItemHelper.cull(VideoItemHelper.cull(matchCollection[index2].Value, "id=\"description-id-", "span"), ": ", "<").Replace(".", "");
                    var url = "http://www.youtube.com/watch?v=" + VideoItemHelper.cull(matchCollection[index2].Value, "watch?v=", "\"");
                    var thumbnail = "https://i.ytimg.com/vi/" + VideoItemHelper.cull(matchCollection[index2].Value, "watch?v=", "\"") + "/mqdefault.jpg";
                    if (title != "__title__" && duration != "")
                        items.Add(new VideoInformation()
                        {
                            Title = title,
                            Author = author,
                            Description = description,
                            Duration = duration,
                            Url = url,
                            Thumbnail = thumbnail
                        });
                }
            }
            return items;
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
                UI.UnhandledException.initWindow(ex, "Error in requesting Video information");
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
                catch
                {
                    Console.WriteLine("getStream Async failed.");
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
                            Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.videoCacheFolder;
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
                Handle.SnackbarWarning("Could not decrypt video");
                return null;
            }
            catch (System.OutOfMemoryException)
            {
                Handle.SnackbarWarning("File too large");
                Console.WriteLine(@"File " + name + @" is to large to save");
            }
            catch (Exception)
            {
                Handle.SnackbarWarning("Failed to cache Video");
                Console.WriteLine(@"Failed to cache file");
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