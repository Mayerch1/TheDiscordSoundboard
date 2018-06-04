using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DiscordBot
{
    public partial class channelIDFavorite : Form
    {
        public List<ChannelFavorite> favorites = new List<ChannelFavorite>();

        public channelIDFavorite(List<ChannelFavorite> _favorites)
        {
            InitializeComponent();

            favorites = _favorites;
            loadListView();
        }

        private void loadListView()
        {
            foreach (var element in favorites)
            {
                ListViewItem item = new ListViewItem(element.name);
                item.SubItems.Add(element.id.ToString());

                listView.Items.Add(item);
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ChannelFavorite newEntry;

            if (ulong.TryParse(idBox.Text, out ulong newId))
            {
                newEntry.id = newId;
                newEntry.name = nameBox.Text;
            }
            else
                return;

            idBox.Text = "";
            nameBox.Text = "";

            if (!favorites.Contains(newEntry) && newEntry.name != "")
            {
                ListViewItem item = new ListViewItem(newEntry.name);
                item.SubItems.Add(newEntry.id.ToString());

                listView.Items.Add(item);

                //listBox.Items.Add(newEntry.name + "\t" + newEntry.id);
                favorites.Add(newEntry);
            }
            nameBox.Focus();
        }

        private void rmBtn_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            var select = listView.SelectedItems[0];
            var sub = select.SubItems[1];

            ChannelFavorite searchItem = new ChannelFavorite();
            searchItem.name = select.Text;
            searchItem.id = ulong.Parse(sub.Text);

            favorites.Remove(searchItem);
            listView.Items.Remove(select);
        }

        private void entryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                addButton_Click(sender, e);
        }

        private void mainPage_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void channelFavorite_ResizeEnd(object sender, EventArgs e)
        {
            // listBox.ColumnWidth = listBox.Width / 2;
        }

        private void listView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void channelIDFavorite_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                rmBtn_Click(sender, e);
            else if (e.KeyCode == Keys.Return)
                addButton_Click(sender, e);
        }
    }
}