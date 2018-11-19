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

namespace DiscordBot.UI.Tutorial
{
#pragma warning disable CS1591
    /// <summary>
    /// Interaction logic for Slide_Bot_2.xaml
    /// </summary>
    public partial class Slide_Bot_2 : UserControl
    {
        public Slide_Bot_2()
        {
            InitializeComponent();
        }

        private void btn_OpenSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(DataManagement.PersistentData.urlToBotRegister);
        }
    }
#pragma warning restore CS1591
}
