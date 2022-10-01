using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using System.IO;
using System.Windows.Media.Imaging;

namespace ClientApp.Core {

    // Source1: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
    // Source2: https://stackoverflow.com/questions/6484357/converting-bitmapimage-to-bitmap-and-vice-versa
    public static class ImageResizer {
        public static BitmapImage Resize(Image image, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            int offset_x = 0, offset_y = 0;

            if (image.Width < image.Height) {
                int diff = image.Height - image.Width;
                offset_y = diff / 2;
            }

            if (image.Width > image.Height) {
                int diff = image.Width - image.Height;
                offset_x = diff / 2;
            }

            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmap)) {
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                using (var attrib = new ImageAttributes()) {
                    attrib.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, offset_x, offset_y, image.Width - (2 * offset_x), image.Height - (2 * offset_y), GraphicsUnit.Pixel, attrib);
                }
            }

            using (var memory = new MemoryStream()) {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmap_image = new BitmapImage();
                bitmap_image.BeginInit();
                bitmap_image.StreamSource = memory;
                bitmap_image.CacheOption = BitmapCacheOption.OnLoad;
                bitmap_image.EndInit();
                bitmap_image.Freeze();

                return bitmap_image;
            }
        }
    }

}