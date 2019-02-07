using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DiscordBot.UI
{
    class DelayedContentResizeGrid: Grid
    {
        private readonly DispatcherTimer Timer = new DispatcherTimer();
        

        public DelayedContentResizeGrid() : base()
        {
            this.SizeChanged += Resizing_Start;
            Timer.Tick += new EventHandler(Resizing_Done);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 75);
        }

        private void Resizing_Mouse_Done(object sender, MouseButtonEventArgs e)
        {
            Resizing_Done(null, null);
        }

        private void Resizing_Start(object sender, SizeChangedEventArgs e)
        {
            Timer.Stop();
            Timer.Start();

            foreach (var child in Children)
            {
                if (child is FrameworkElement fe)
                {
                    fe.Width = fe.ActualWidth;
                    fe.Height = fe.ActualHeight;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }       
        }

        private void Resizing_Done(object sender, EventArgs e)
        {
            Timer.Stop();

            foreach (var child in Children)
            {

                if (child is FrameworkElement fe)
                {
                    fe.Width = Double.NaN;
                    fe.Height = Double.NaN;
                    fe.VerticalAlignment = VerticalAlignment.Stretch;
                    fe.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
            }
        }
    }
}
