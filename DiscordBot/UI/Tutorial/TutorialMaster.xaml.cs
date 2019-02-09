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
using MaterialDesignThemes.Wpf.Transitions;

namespace DiscordBot.UI.Tutorial
{
#pragma warning disable CS1591
    /// <summary>
    /// Interaction logic for TutorialMaster.xaml
    /// </summary>
    public partial class TutorialMaster : UserControl
    {
        public delegate void TutorialFinishedHandle();

        public TutorialFinishedHandle TutorialFinished;


        public TutorialMaster()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Transitioner element)
            {

                var item = element.SelectedItem;
                RegisterSlideEvents(item);
            }
            
        }

        private void RegisterSlideEvents(object obj)
        {
            switch (obj)
            {
                case Slide_EnterCredentials sl:
                    sl.FinishedSetup += FinishSetup;
                    break;
            }
        }

        private void FinishSetup(bool isOk)
        {
            if (isOk)
                TutorialFinished();
        }
    }
#pragma warning restore CS1591
}
