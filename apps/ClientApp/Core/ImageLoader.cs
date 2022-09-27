using System.IO;
using System.Windows.Media.Imaging;

namespace ClientApp.Core {

    public static class ImageLoader {
        public static BitmapImage Load(byte[] data) {
            var image = new BitmapImage();

            using (var ms = new MemoryStream(data)) {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
            }

            return image;
        }
    }

}