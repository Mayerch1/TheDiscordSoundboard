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
        private const string imageUrl = "https://img.youtube.com/vi/";

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
        /// <returns>direct url to thumbnail</returns>
        public static string getUrlToThumbnail(string url)
        {
            return imageUrl + getIdFromUrl(url) + thumbnailQuality;
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
            try
            {
                mpAudio = await yt.GetVideoAsync(url);

                ////get video file
                //var videos = await YouTube.Default.GetAllVideosAsync(url);

                ////get audios, only aac
                //var audios = videos.Where(v => v.AudioFormat == AudioFormat.Aac && v.AdaptiveKind == AdaptiveKind.Audio).ToList();

                ////save audio into Video, only with audio
                //mpAudio = audios.FirstOrDefault(x => x.AudioBitrate > 0);

                Console.WriteLine(mpAudio.Uri);
            }
            catch (Exception ex)
            {
                UI.UnhandledException.initWindow(ex, "Error in downloading Video");
                Console.WriteLine("Exception.!?<");

                return null;
            }
            return mpAudio;
        }

        /// <summary>
        /// saves a video into cache folder, this may block ui thread when disc is slow
        /// </summary>
        /// <param name="vid">video to save</param>
        public async static Task<string> cacheVideo(Video vid)
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
                File.WriteAllBytes(location, await vid.GetBytesAsync());

                Console.WriteLine("Succesfully saved file");
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Handle.SnackbarWarning("Could not decrypt video");
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in saving file");

                return null;
            }

            return location;
        }
    }
}