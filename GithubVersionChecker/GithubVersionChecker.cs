using System;
using System.Net;
using System.Threading.Tasks;

namespace GithubVersionChecker
{
    /// <summary>
    /// Sets for which Version-step to look for
    /// </summary>
    public enum VersionChange
    {
        /// <summary>
        /// Major software update (X.0.0.0)
        /// </summary>
        Major,

        /// <summary>
        /// Minor software update (0.X.0.0)
        /// </summary>
        Minor,

        /// <summary>
        /// Build change (0.0.X.0)
        /// </summary>
        Build,

        /// <summary>
        /// Revision changed (0.0.0.X)
        /// </summary>
        Revision
    }

    /// <summary>
    /// Checks if your github repo is on a newer version or not
    /// </summary>
    public class GithubUpdateChecker
    {
        private const string githubUrl = "https://github.com/";
        private const string latestVersion = "/releases/latest";

        private string Username;
        private string Repository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Username">Username of Repository owner</param>
        /// <param name="Repository">Name of Github Repository</param>
        public GithubUpdateChecker(string Username, string Repository)
        {
            this.Username = Username;
            this.Repository = Repository;
        }

        /// <summary>
        /// This checks, if an update is available on the github repo, is awaitable
        /// </summary>
        /// <param name="CurrentVersion">the current version of the programm in form of 1.0.0.0</param>
        /// <param name="VersionChange">for which of the changed enum an update should be offered</param>
        /// <returns>bool, if an update is available</returns>
        public async Task<bool> CheckForUpdateAsync(string CurrentVersion, VersionChange VersionChange = VersionChange.Minor)
        {
            string resolved = await getResponseUrlAsync(githubUrl + Username + "/" + Repository + latestVersion);

            if (resolved != null)
                return compareVersions(CurrentVersion, resolved, VersionChange);
            else
                return false;
        }

        /// <summary>
        /// This checks, if an update is available on the github repo
        /// </summary>
        /// <param name="CurrentVersion">the current version of the programm in form of 1.0.0.0</param>
        /// <param name="VersionChange">for which of the changed enum an update should be offered</param>
        /// <returns>bool, if an update is available</returns>
        public bool CheckForUpdate(string CurrentVersion, VersionChange VersionChange = VersionChange.Minor)
        {
            string resolved = getResponseUrl(githubUrl + Username + "/" + Repository + latestVersion);

            if (resolved != null)
                return compareVersions(CurrentVersion, resolved, VersionChange);
            else
                return false;
        }

        private bool compareVersions(string current, string github, VersionChange changeLevel)
        {
            //no releases yet
            if (!github.Contains("/tag/"))
                return false;

            //get everything after last /tag/
            github = github.Substring(github.LastIndexOf("/tag/") + 5);

            //separate version into VersionChange
            var currentArr = current.Split('.');
            var gitArr = github.Split('.');

            //arrs for int of arrays above
            int[] intCurrent = new int[] { 0, 0, 0, 0 };
            int[] intGit = new int[] { 0, 0, 0, 0 };

            //convert all versions to int
            for (int i = 0; (i < intCurrent.Length) && (i<currentArr.Length); i++)
            {
                intCurrent[i] = int.Parse(currentArr[i]);
            }
            //convert all git version to int
            for (int i = 0; (i < intGit.Length) && (i < gitArr.Length); i++)
            {
                intGit[i] = int.Parse(gitArr[i]);
            }

            //check for major change
            if (intCurrent[0] < intGit[0])
            {
                return true;
            }
            else if (intCurrent[0] == intGit[0] &&
                (changeLevel == VersionChange.Minor || changeLevel == VersionChange.Build || changeLevel == VersionChange.Revision))
            {
                //change for minor change
                if (intCurrent[1] < intGit[1])
                {
                    return true;
                }
                else if (intCurrent[1] == intGit[1] &&
                    (changeLevel == VersionChange.Build || changeLevel == VersionChange.Revision))
                {
                    //check for build change
                    if (intCurrent[2] < intGit[2])
                    {
                        return true;
                    }
                    else if (intCurrent[2] == intGit[2])
                    {
                        //check for revision change
                        if (intCurrent[3] < intGit[3])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private string getResponseUrl(string request)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request);
            WebResponse wResp;
            try
            {
                wResp = req.GetResponse();
            }
            catch
            {
                //if not available
                return null;
            }

            return wResp.ResponseUri.ToString();
        }

        private async Task<string> getResponseUrlAsync(string request)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request);
            WebResponse wResp;
            try
            {
                wResp = await req.GetResponseAsync();
            }
            catch
            {
                return null;
                //if not available
            }

            return wResp.ResponseUri.ToString();
        }
    }
}