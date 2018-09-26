using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace DicsordBot.IO
{
    /// <summary>
    /// Manages Image operations
    /// </summary>
    public static class ImageManager
    {
        /// <summary>
        /// copys image into appdata, downsamples it first
        /// </summary>
        /// <param name="imagePath">path to image</param>
        public static string cacheImage(string imagePath)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.imageCacheFolder;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var scaledImage = ResizeImage(new Bitmap(imagePath), 256, 256);

            var location = folder + @"\" + System.IO.Path.GetFileName(imagePath);
            try
            {
                scaledImage.Save(location);
            }
            catch { return null; }

            return location;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRec = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    //TODO: cut of, if not squared
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Clamp);
                    graphics.DrawImage(image, destRec, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        /// <summary>
        /// clears the cache folder, for unused images
        /// </summary>
        /// <param name="playlists">playists with all cached images as property</param>
        public static void clearImageCache(ObservableCollection<Data.Playlist> playlists)
        {
            List<string> usedImages = new List<string>();
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Data.PersistentData.defaultFolderName + @"\" + Data.PersistentData.imageCacheFolder;
            string[] files = Directory.GetFiles(folder);

            foreach (var list in playlists)
            {
                usedImages.Add(list.ImagePath);
            }

            foreach (var image in files)
            {
                if (!usedImages.Contains(image))
                {
                    try
                    {
                        File.Delete(image);
                    }
                    catch { }
                }
            }
        }
    }
}