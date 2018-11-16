using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Util.IO
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
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + DataManagement.PersistentData.defaultFolderName + @"\" + DataManagement.PersistentData.imageCacheFolder;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var scaledImage = ResizeImage(new Bitmap(imagePath), 256);

            var location = folder + @"\" + System.IO.Path.GetFileName(imagePath);
            try
            {
                scaledImage.Save(location);
            }
            catch
            {
                //return location as it would succedd, if it is already existing
                if (!File.Exists(location))
                {
                    return null;
                }
            }

            return location;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to = The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width)
        {
            int height = width;
            //target dimension
            var destRec = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                //quality settings
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                //crop image to square
                int t = 0, l = 0;
                if (image.Height > image.Width)
                    t = (image.Height - image.Width) / 2;
                else
                    l = (image.Width - image.Height) / 2;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Clamp);
                    //apply settings for offest to achieve square-crop
                    graphics.DrawImage(image, destRec, l, t, image.Width - l * 2, image.Height - t * 2, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        /// <summary>
        /// clears the cache folder, for unused images
        /// </summary>
        /// <param name="playlists">playists with all cached images as property</param>
        public static void clearImageCache(ObservableCollection<DataManagement.Playlist> playlists)
        {
            List<string> usedImages = new List<string>();
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + DataManagement.PersistentData.defaultFolderName + @"\" + DataManagement.PersistentData.imageCacheFolder;
            string[] files;
            try
            {
                files = Directory.GetFiles(folder);
            }
            catch {/* dirctory not accessible -> no operation possible */ return; }

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