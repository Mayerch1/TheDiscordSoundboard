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
        public MyButton[] BtnList { get; set; }

        public ButtonUI()
        {
            InitializeComponent();

            BtnList = new MyButton[11];

            for (int i = 0; i < 11; i++)
            {
                var btn = new MyButton("Test-" + i);
                btn.ID = i;

                BtnList[i] = (btn);
            }

            btnControl.ItemsSource = BtnList;

            this.DataContext = this;
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Console.WriteLine(btn.Tag);

            int index = (int)btn.Tag;

            BtnList[index].Name = "Clicked";
            btn.Content = "Clicked";
            Handle.Bot.disconnectFromServerAsync();
        }
    }

    public class MyButton
    {
        public MyButton(string n)
        {
            Name = n;
        }

        public string Name { get; set; }
        public int ID { get; set; }
    }
}