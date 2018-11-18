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
    /// Interaction logic for Slide_Bot_5.xaml
    /// </summary>
    public partial class Slide_Bot_5 : UserControl
    {
        public Slide_Bot_5()
        {
            InitializeComponent();
        }

        private void FirstSlideButton_OnClick(object sender, RoutedEventArgs e)
        {
            Transitioner.SelectedIndex = 0;
        }

        private void SecondSlideButton_OnClick(object sender, RoutedEventArgs e)
        {
            Transitioner.SelectedIndex = 1;
        }
    }
#pragma warning restore CS1591
}
