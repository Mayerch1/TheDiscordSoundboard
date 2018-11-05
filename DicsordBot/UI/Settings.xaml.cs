using System;
using System.Windows;
using System.Windows.Controls;

namespace DiscordBot.UI
{
#pragma warning disable CS1591

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        /// <summary>
        /// constructor for Settings class, sets Datacontext
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            this.DataContext = Handle.Data.Persistent;
        }

        /// <summary>
        /// eventhandler for changed text in the bot-token box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void box_token_TextChanged(object sender, TextChangedEventArgs e)
        {
            Handle.Token = ((TextBox)sender).Text;
        }

        private void btn_Help_Application_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#application");
        }

        private void btn_Help_Files_Click(object sender, RoutedEventArgs e)
        {
            openHelpPage("Settings#files");
        }

        private void openHelpPage(string page)
        {
            System.Diagnostics.Process.Start(Data.PersistentData.urlToGitRepo + "wiki/" + page);
        }

        private void box_userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;

            //replace all blancs
            box.Text = box.Text.Replace(" ", String.Empty);

            box.SelectionStart = box.Text.Length;
            box.SelectionLength = 0;
        }

        private void btn_addMediaSource_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                //show dialog to add a new media-source
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    //add selected source to list
                    if (!Handle.Data.Persistent.MediaSources.Contains(dialog.SelectedPath))
                        Handle.Data.Persistent.MediaSources.Add(dialog.SelectedPath);

                    var scanCollection = new System.Collections.ObjectModel.ObservableCollection<string>();
                    scanCollection.Add(dialog.SelectedPath);

                    //rescan added files
                    IO.FileWatcher.indexFiles(scanCollection, false);
                }
            }
        }

        private void btn_deleteMediaSource_Click(object sender, RoutedEventArgs e)
        {
            if (list_MediaSources.SelectedItems.Count > 0)
            {
                string sPath = list_MediaSources.SelectedItem.ToString();

                int index = Handle.Data.Persistent.MediaSources.IndexOf(sPath);

                if (index >= 0)
                {
                    Handle.Data.Persistent.MediaSources.RemoveAt(index);

                    //delete/recsan all files
                    IO.FileWatcher.indexCleanFiles(Handle.Data.Persistent.MediaSources);
                }
            }
        }
    }

#pragma warning restore CS1591
}