using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Playground.Web.Util
{
    public static class ImageUtil
    {
        public static void ScaleImage(string sourcePath, string destinationPath, System.Drawing.Imaging.ImageFormat format, int maxWidth, int maxHeight)
        {
            using (Image image = Image.FromFile(sourcePath))
            {
                var ratio = (double)1;
                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    var ratioX = (double)maxWidth / image.Width;
                    var ratioY = (double)maxHeight / image.Height;
                    ratio = Math.Min(ratioX, ratioY);
                }
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.DrawImage(image, 0, 0, newWidth, newHeight);
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                    newImage.Save(destinationPath, format);
                }
            }
        }

        public static void CropImage(string sourcePath, string destinationPath, System.Drawing.Imaging.ImageFormat format, CropingCoords cropingCoords)
        {
            using (Image image = Image.FromFile(sourcePath))
            {
                var newImage = new Bitmap(cropingCoords.H, cropingCoords.W);
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.DrawImage(image, new Rectangle(0, 0, cropingCoords.W, cropingCoords.H),
                                        new Rectangle(cropingCoords.X, cropingCoords.Y, cropingCoords.W, cropingCoords.H),
                                        GraphicsUnit.Pixel);
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                    newImage.Save(destinationPath, format);
                }
            }
        }
    }
}