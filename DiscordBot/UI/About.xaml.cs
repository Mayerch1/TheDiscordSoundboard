using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

namespace DiscordBot.UI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {

        private readonly List<License> thirdParty = new List<License>()
        {
            new License("Concentus", "Opus License", @"https://opus-codec.org/license/"),
            new License("Discord.Net", "MIT License", @"https://opensource.org/licenses/MIT"),
            new License("Gong-WPF-DragDrop", "BSD 3-Clause", @"https://github.com/punker76/gong-wpf-dragdrop/blob/dev/LICENSE"),
            new License("MaterialDesign for XML", "MIT License", @"https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/LICENSE"),
            new License("NAudio", "Microsoft Public License", @"https://github.com/naudio/NAudio/blob/master/license.txt"),
            new License("Soundtouch(.Net)", "LGPL 2.1", @"https://raw.githubusercontent.com/owoudenberg/soundtouch.net/master/LICENSE"),
            new License("Soundtouch.Net.NAudio", "LGPL 2.1", @"https://raw.githubusercontent.com/owoudenberg/soundtouch.net/master/LICENSE"),
            new License("NYoutubeDL", "MIT License", @"https://licenses.nuget.org/MIT"),
            new License("libvideo", "BSD 2-Clause", @"https://github.com/i3arnon/libvideo/blob/master/LICENSE"),
            new License("YoutubeSearch", "GPL License", @"https://github.com/mrklintscher/YoutubeSearch#license"),
            new License("Taglib", "LGPL 2.1", @"https://opensource.org/licenses/lgpl-2.1.php"),
            new License("Newtonsoft.Json", "MIT License", @"https://licenses.nuget.org/MIT"),
            new License("Microsoft .NET Library", "Microsoft", @"https://dotnet.microsoft.com/dotnet_library_license.htm"),
        };

        public List<License> ThirdParty => thirdParty;


        /// <summary>
        /// constructor of class
        /// </summary>
        public About()
        {
            InitializeComponent();
            this.DataContext = this;


        }

        private void btn_Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/CJMayer/2,99");
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

    public class License
    {
        public License(string name, string model, string url=null)
        {
            Name = name;
            Licensemodel = model;

            Url = url ?? Licensemodel;
        }


        public string Name { get; set; }
        public string Licensemodel { get; set; }
        public string Url { get; set; }
    }
}