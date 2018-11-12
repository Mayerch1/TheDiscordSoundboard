// This file is part of YoutubeSearch.
//
// YoutubeSearch is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// YoutubeSearch is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with YoutubeSearch. If not, see<http://www.gnu.org/licenses/>.

namespace YoutubeSearch
{
    /// <summary>
    /// Helper Class for <see cref="VideoSearch"/>
    /// </summary>
    public class VideoItemHelper
    {
        /// <summary>
        /// extracts information out of string (using strings as delimiter)
        /// </summary>
        /// <param name="strSource">source</param>
        /// <param name="strStart">start of Substring</param>
        /// <param name="strEnd">end of Substring</param>
        /// <returns>Part of string between Start and End. "" if no matches</returns>
        /// <remarks>wraps Substring method into error fallback and ability to separate for strings instead of indexes</remarks>
        /// <seealso cref="string.Substring(int, int)"/>
        /// <returns></returns>
        public static string cull(string strSource, string strStart, string strEnd)
        {
            int Start, End;

            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);

                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }
}
