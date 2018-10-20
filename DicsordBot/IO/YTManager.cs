using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

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

            VideoClient videoClient = new VideoClient();

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
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.videoCacheFolder;
            //save video into cache folder
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //compute hash
           

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
            catch (Exception ex)
            {
                Handle.SnackbarWarning("Failed to cache Video");
                return null;
            }

            return location;
        }


        private static string getHashSha256(string title)
        {
            byte[] byteStr = Encoding.UTF8.GetBytes(title);

            byte[] hash = new SHA256Managed().ComputeHash(byteStr);

            string hashStr = String.Empty;


            foreach (var b in hash)
            {
                hashStr += b.ToString("x2");
            }
  
            return hashStr;
        }

    }
}