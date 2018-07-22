using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ButtonUI.xaml
    /// </summary>
    public partial class ButtonUI
    {
        public ButtonUI()
        {
            InitializeComponent();

            Handle.Data.resizeBtnList();

            btnControl.ItemsSource = Handle.Data.Persistent.BtnList;

            this.DataContext = this;
        }

        private void btn_Instant_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            int index = (int)btn.Tag;
            btn.Content = "Clicked";


            

            execBtn(index);
        }


        private async void execBtn(int index)
        {
            await Handle.Bot.enqueueAsync(Handle.Data.Persistent.BtnList[index]);

        }


    }
}