using System;
using System.Net;
using System.Threading.Tasks;

namespace DicsordBot
{
    /// <summary>
    /// Checks for upgrades and opens the download page
    /// </summary>
    public class UpdateChecker
    {
        /// <summary>
        /// Checks if the version number differs from the latest release on github
        /// </summary>
        /// <returns>Returns if version number differs from latest release</returns>
        public static async Task<bool> CheckForUpdate()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Data.PersistentData.urlToGitRepo + "releases/latest");
            WebResponse wResp = null;
            try
            {
                wResp = await req.GetResponseAsync();
            }
            catch (System.Net.WebException)
            {
                //if not available, assume there are no updates
                return false;
            }

            HttpWebResponse resp = (HttpWebResponse)wResp;

            string resolved = resp.ResponseUri.ToString();

            //this takes any published version as update, disregarding version numbers
            //cuts out everythis but the last part after the last '/'
            if (resolved.Substring(resolved.LastIndexOf('/')).Substring(1) != Data.PersistentData.version)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Opens Github-page with download for latest release
        /// </summary>
        public static void OpenUpdatePage()
        {
            System.Diagnostics.Process.Start(Data.PersistentData.urlToGitRepo + "releases/latest");
        }
    }
}