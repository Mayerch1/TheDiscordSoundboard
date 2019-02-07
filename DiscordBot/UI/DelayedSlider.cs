using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DiscordBot.UI
{
    class DelayedSlider : System.Windows.Controls.Slider
    {
        private readonly DispatcherTimer Timer = new DispatcherTimer();
        private double oldValue, newValue;


        public event RoutedPropertyChangedEventHandler<double> DelayedValueChanged;


        public DelayedSlider() : base()
        {
            Timer.Tick += new EventHandler(FireDelayedValueChanged);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 125);

            this.ValueChanged += ValueChangedListen;
        }

        private void ValueChangedListen(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            oldValue = e.OldValue;
            newValue = e.NewValue;

            Timer.Stop();
            Timer.Start();
        }

        private void FireDelayedValueChanged(object sender, EventArgs e)
        {
            Timer.Stop();
            OnDelayedValueChanged(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
        }

        protected virtual void OnDelayedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DelayedValueChanged?.Invoke(sender, e);
        }
    }
}
