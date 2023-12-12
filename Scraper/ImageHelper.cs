using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Scraper
{
    internal static class ImageHelper
    {
        public static byte[] DownsizeImage(byte[] imageBytes)
        {
            Image image;
            int scale = 1;

            using (var ms = new MemoryStream(imageBytes))
            {
                image = Image.FromStream(ms);
            }


            if (image.Height > 900 || image.Width > 900)
            {
                scale = 3;
            }
            else if (image.Height > 500 || image.Width > 500)
            {
                scale = 2;
            }

            int resH = image.Height / scale;
            int resW = image.Width / scale;
            
            var destRect = new Rectangle(0, 0, resW, resH);
            var destImage = new Bitmap(resW, resH);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            using (var ms = new MemoryStream())
            {
                destImage.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                byte[] data = ms.ToArray();
                return data;
            }
        }
    }
}
