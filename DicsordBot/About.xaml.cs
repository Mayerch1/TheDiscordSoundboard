using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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