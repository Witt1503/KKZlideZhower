using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KKZlideZhower
{
    internal class ImageViewer:IViewer
    {
        private string file;
        private Image myImage;
        private TextBlock Overlay;

        public ImageViewer(string file, Image img, TextBlock Overlay)
        {
            this.file = file;
            this.myImage = img;
            this.Overlay = Overlay;
        }
                public void view()
        {
            var txt = file.Split('\\');
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(file, UriKind.Absolute);
            image.EndInit();
            myImage.Source = image;
            myImage.Stretch = Stretch.Fill;
            myImage.StretchDirection = StretchDirection.Both;
            Overlay.Text = txt[txt.Length - 2] == "Reklamer" ? "" : txt[txt.Length - 2];
        }
    }
}