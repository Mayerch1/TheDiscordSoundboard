using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DicsordBot
{
    internal class ChannelListManager
    {
        private const int maxTitleLen = 30;

        public List<SocketVoiceChannel> ChannelList { get; set; }
        private List<List<SocketVoiceChannel>> CompleteList { get; set; }

        public ChannelListManager()
        {
            ChannelList = new List<SocketVoiceChannel>();
        }

        public async Task initAsync()
        {
            //get all channels from all servers
            CompleteList = await Handle.Bot.getAllChannels();
            //prevent crash, when not connected
            if (CompleteList != null)
            {
                //sort channels per server
                for (int i = 0; i < CompleteList.Count; i++)
                {
                    CompleteList[i] = CompleteList[i].OrderBy(o => o.Position).ToList();
                }
            }
        }

        public void populateTree(TreeView tree)
        {
            tree.Items.Clear();

            //0 lets the bot search and join for the owner
            tree.Items.Add(new MyTreeItem("Join to owner", 0));

            if (CompleteList != null && CompleteList.Count > 0 && CompleteList[0].Count > 0)
            {
                //add all servers
                for (int i = 0; i < CompleteList.Count; i++)
                {
                    var server = CompleteList[i];

                    TreeViewItem newBranch = new TreeViewItem();

                    //cut of to long names
                    string serverName;
                    if (server[0].Guild.ToString().Length > maxTitleLen)
                    {
                        serverName = server[0].Guild.ToString().Substring(0, maxTitleLen) + "...";
                    }
                    else
                        serverName = server[0].Guild.ToString();

                    newBranch.Header = serverName;
                    newBranch.Tag = i;

                    //add channels of  server
                    foreach (var channel in server)
                    {
                        //cut of to long names
                        string name;
                        if (channel.Name.Length > maxTitleLen)
                        {
                            name = channel.Name.Substring(0, maxTitleLen) + "...";
                        }
                        else
                            name = channel.Name;

                        MyTreeItem newElement = new MyTreeItem(name, channel.Id);

                        newBranch.Items.Add(newElement);
                    }

                    //expand last opened server
                    if (Handle.Data.Persistent.SelectedServerIndex == i)
                        newBranch.IsExpanded = true;

                    tree.Items.Add(newBranch);
                }
            }
            else
            {
                TreeViewItem errorBranch = new TreeViewItem();
                errorBranch.Header = "Could not load channels";
                errorBranch.Items.Add("Check Token");
                errorBranch.Items.Add("Check Permissions");
                errorBranch.Items.Add("Check Status of Discord Server");
                tree.Items.Add(errorBranch);
            }
        }
    }

    internal struct MyTreeItem
    {
        public MyTreeItem(string n, ulong _id)
        {
            id = _id;
            name = n;
        }

        public override string ToString()
        {
            return name;
        }

        public ulong id;
        public string name;
    }
}