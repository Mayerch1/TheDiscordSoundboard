using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicsordBot
{
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

        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Data.PersistentData.urlToGitRepo + "wiki/Settings#application");
        }
    }
}