using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KKZlideZhower
{
    internal class ImageViewer:IViewer
    {
        private string file;
        public bool isReklame;
        private MainWindow main;

        public ImageViewer(string file, MainWindow main)
        {
            this.file = file;
            this.main = main;
            var txt = file.Split('\\');
            isReklame = txt[txt.Length - 2] == "Reklamer";
        }

        public TimeSpan time { get; set; }

        public void view()
        {
            var txt = file.Split('\\');
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(file, UriKind.Absolute);
            image.EndInit();
            main.myImage.Source = image;
            main.myImage.Stretch = Stretch.Fill;
            main.myImage.StretchDirection = StretchDirection.Both;
            main.Overlay.Text = isReklame ? "" : txt[txt.Length - 2];
        }
    }
}