namespace DataManagement
{
#pragma warning disable CS1591

    /// <summary>
    /// Contains data for streaming
    /// </summary>
    public class BotData
    {
        public BotData()
        {
        }

        public BotData(string name, string filePath, string uri = "", string deviceId = "", string author = "")
        {
            this.name = name;
            this.filePath = filePath;
            this.uri = uri;
            this.author = author;
            this.deviceId = deviceId;
        }


        public BotData(ButtonData btn)
        {
            name = btn.Name;
            filePath = btn.File;
            isEarrape = btn.IsEarrape;
            isLoop = btn.IsLoop;
            id = btn.ID;
            uri = "";
            deviceId = "";
            author = btn.Author;
        }

        public string name = "";
        public string uri = "";
        public string deviceId = "";
        public string filePath = null;
        public bool isEarrape = false;
        public bool isLoop = false;
        public int id = -1;
        public string author = "";
    }

#pragma warning restore CS1591
}