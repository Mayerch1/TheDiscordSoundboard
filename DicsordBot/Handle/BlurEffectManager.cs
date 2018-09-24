using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace DicsordBot
{
    /// <summary>
    /// Handles blur and fade-out effect for any window
    /// </summary>
    public static class BlurEffectManager
    {
        /// <summary>
        /// delegate for telling another class, that blur effect should be changed
        /// </summary>
        /// <param name="isEnabled"></param>
        public delegate void ToggleBlurEffectHandler(bool isEnabled);

        /// <summary>
        /// ToggleBlurEffectHandler for telling another class, that blur effect should be changed
        /// </summary>
        public static ToggleBlurEffectHandler ToggleBlurEffect;

        /// <summary>
        /// This toggles the blur and fade-out effect of an window
        /// </summary>
        /// <param name="isEnabled">toggle effect on or off</param>
        /// <param name="window">window to apply effect on</param>
        public static void ApplyBlurEffect(bool isEnabled, MainWindow window)

        {
            if (isEnabled)
            {
                window.Effect = new System.Windows.Media.Effects.BlurEffect();
                window.Opacity = 0.8;
            }
            else
            {
                window.Effect = null;
                window.Opacity = 1;
            }
        }
    }
}