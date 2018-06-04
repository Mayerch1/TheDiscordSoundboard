using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DiscordBot
{
    public partial class msgPresetWindow : Form
    {
        public List<StatusMessage> statusMessages = new List<StatusMessage>();

        public msgPresetWindow(List<StatusMessage> _statusMessages)
        {
            InitializeComponent();
            statusMessages = _statusMessages;
            loadListView();
        }

        private void loadListView()
        {
            foreach (var element in statusMessages)
            {
                ListViewItem item = new ListViewItem(element.name);
                item.SubItems.Add(element.url);

                listView.Items.Add(item);
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            StatusMessage newEntry;

            newEntry.url = urlBox.Text;
            newEntry.name = nameBox.Text;

            urlBox.Text = "";
            nameBox.Text = "";

            if (!statusMessages.Contains(newEntry) && newEntry.name != "")
            {
                ListViewItem item = new ListViewItem(newEntry.name);
                item.SubItems.Add(newEntry.url);

                listView.Items.Add(item);

                //listBox.Items.Add(newEntry.name + "\t" + newEntry.id);
                statusMessages.Add(newEntry);
            }
            nameBox.Focus();
        }

        private void rmBtn_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            var select = listView.SelectedItems[0];
            var sub = select.SubItems[1];

            StatusMessage searchItem = new StatusMessage();
            searchItem.name = select.Text;
            searchItem.url = sub.Text;

            statusMessages.Remove(searchItem);
            listView.Items.Remove(select);
        }

        private void mainPage_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                rmBtn_Click(sender, e);
            if (e.KeyCode == Keys.Return)
                addButton_Click(sender, e);
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
    }
}