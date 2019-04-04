using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.com.chartlyrics.api;

namespace Util.IO
{
    /// <summary>
    /// wraps the SOAP web ref. to lyrics database
    /// </summary>
    public class LyricsManager
    {
        private static string _title=null, _author=null;


        public static GetLyricResult GetLyrics(int lyricId, string lyricChecksum)
        {
            var request = new com.chartlyrics.api.apiv1();
            return request.GetLyric(lyricId, lyricChecksum);
        }

        /// <summary>
        /// get the lyrics of requested song
        /// </summary>
        /// <param name="title">title of song</param>
        /// <param name="author">author of song</param>
        /// <returns>"" if no result</returns>
        public static SearchLyricResult[] queryResultList(string title=null, string author=null)
        {
            if (title != null)
                _title = title;
            if (author != null)
                _author = author;


            if (_title == null || _author == null)
                return null;

            var request = new com.chartlyrics.api.apiv1();

            SearchLyricResult[] result;

            try
            {
                result = request.SearchLyric(_author, _title);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            if (result.Length > 0 && result[0] != null)
            {
                //get min(result.Length, 5)
                int count = result.Length > 5 ? 5 : result.Length;

               
                return result.Take(count).ToArray();
                //request.GetLyricAsync(result[0].LyricId, result[0].LyricChecksum);
                try
                {
                    var lyr = request.GetLyric(result[0].LyricId, result[0].LyricChecksum);
                    //return lyr;
                }
                catch (Exception)
                {
                    return null;
                }


            }
            return null;
        }

        /// <summary>
        /// Set the parameter for future API-request
        /// </summary>
        /// <param name="title">title of song</param>
        /// <param name="author">author of song</param>
        public static void setParameter(string title, string author)
        {
            if (title != null)
                _title = title;
            if (author != null)
                _author = author;
        }

    }
}
