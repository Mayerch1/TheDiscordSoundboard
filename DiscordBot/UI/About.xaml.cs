using System.Windows;
using System.Windows.Controls;

namespace DiscordBot.UI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        /// <summary>
        /// constructor of class
        /// </summary>
        public About()
        {
            InitializeComponent();
            this.DataContext = Handle.Data.Persistent;
        }

        private void btn_Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/CJMayer/4,99");
        }

        private void btn_license_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("GNU_LICENSE.rtf");
            }
            catch
            {
                try
                {
                    System.Diagnostics.Process.Start("https://www.gnu.org/licenses/gpl-3.0.de.html");
                }
                catch
                {
                    return;
                }
            }
        }
    }
}