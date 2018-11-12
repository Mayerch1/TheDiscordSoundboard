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
    /// Information storage for search result of <see cref="VideoSearch"/>
    /// </summary>
    public class VideoInformation
    {
#pragma warning disable CS1591
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
#pragma warning restore CS1591
    }
}
