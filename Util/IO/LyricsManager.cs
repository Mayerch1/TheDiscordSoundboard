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
        /// <summary>
        /// get the lyrics of requested song
        /// </summary>
        /// <param name="title">title of song</param>
        /// <param name="author">author of song</param>
        /// <returns>"" if no result</returns>
        public static string getLyrics(string title, string author)
        {
            var request = new com.chartlyrics.api.apiv1();

            SearchLyricResult[] result;

            try
            {
                result = request.SearchLyric(author, title);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }

            if (result.Length > 0)
            {
                var lyricResult = request.GetLyric(result[0].LyricId, result[0].LyricChecksum);
                return lyricResult.Lyric;
            }
            return "";
        }     
    }
}
