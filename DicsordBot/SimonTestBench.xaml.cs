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
    /// Interaction logic for SimonTestBench.xaml
    /// </summary>
    public partial class SimonTestBench : UserControl
    {
        #region fields
        private bool placeHolder;
        # endregion

        #region propertys

        public bool IsLoading { get; set; }

        #endregion propertys

        public SimonTestBench()
        {
            InitializeComponent();
            registerEvents();
        }

        private void registerEvents()
        {
            //event Handler for Stream-state of bot
            Handle.Bot.StreamStateChanged += delegate (bool newState)
            {
            };
        }
    }
}