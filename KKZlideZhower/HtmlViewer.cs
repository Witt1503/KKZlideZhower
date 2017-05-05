using KKZlideZhower;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace KKZlideZhower
{
    internal class HtmlViewer : IViewer
    {
        public string data { get; set; }
        public DateTime timeStamp;
        private MainWindow main;
        private string rootDir;
        private Regex regDate = new Regex(@"\d{2}[\/]\d{2}[\/]\d{4}\s+\d{2}[\:]\d{2}");
        private BitmapSource currentImage;

        public HtmlViewer(MainWindow main, string rootDir)
        {
            this.main = main;
            this.rootDir = rootDir;
            File.Delete(rootDir + @"\Reklamer\lvlUp.jpg");
            CreateJpgFromHtml();
            
        }


        public TimeSpan time
        { get; set; }

        public void view()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            while (!File.Exists(rootDir + @"\Reklamer\lvlUp.jpg")) { }
            image.UriSource = new Uri(rootDir + @"\Reklamer\lvlUp.jpg", UriKind.Absolute);
            image.EndInit();
            main.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 250));
            main.myImage.Source = image;
            main.myImage.Stretch = System.Windows.Media.Stretch.Uniform;
            main.myImage.StretchDirection = System.Windows.Controls.StretchDirection.Both;
            main.Overlay.Text = "";



        }
        public void pullLvlUpData() // depricated. Kept to be able to reproduce.
        {
            var url = @"https://dl.dropboxusercontent.com/s/6ouufk1g0808thd/LvlUp_F17.csv";
            string data = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                this.data = reader.ReadToEnd();
            }

        }
        public DataTable CsvDb(string filename, string separatorChar) // depricated. Kept to be able to reproduce.
        {
            var table = new DataTable("Filecsv");
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                string line;
                var i = 0;
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line) || regDate.IsMatch(line)) continue;
                        var values = line.Split(new[] { separatorChar }, StringSplitOptions.None);
                        var row = table.NewRow();
                        for (var colNum = 0; colNum < values.Length; colNum++)
                        {
                            var value = values[colNum];
                            if (i == 0)
                            {
                                table.Columns.Add(value, typeof(String));
                            }
                            else
                            { row[table.Columns[colNum]] = value; }
                        }
                        if (i != 0) table.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        string cErr = ex.Message;
                        //if you need the message error
                    }
                    i++;
                }
            }
            return table;
        }

        void CreateJpgFromHtml()
        {
            var source = "";
            using (StreamReader sr = new StreamReader(rootDir + @"\lvlup.html"))
            {
                // Read the stream to a string, and write the string to the console.
                source = sr.ReadToEnd();
            }

            StartBrowser(source);
        }

        private void StartBrowser(string source)
        {
            var th = new Thread(() =>
            {
                var webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.DocumentTitleChanged += WebBrowser_DocumentTitleChanged;
                //+= webBrowser_DocumentCompleted;
                webBrowser.DocumentText = source;
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void WebBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            var webBrowser = (WebBrowser)sender;
            if (webBrowser.DocumentTitle != "done") return;
            //webBrowser.Width = 600;
            //webBrowser.Height = 900;
            var newSize = webBrowser.Document.Body.ScrollRectangle.Size;
            newSize.Height = newSize.Height + 20;
            newSize.Width = newSize.Width + 20;
            webBrowser.Size = newSize;
            using (Bitmap bitmap =
                new Bitmap(
                    webBrowser.Width,
                    webBrowser.Height))
            {
                webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                currentImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(webBrowser.Width, webBrowser.Height));
                bitmap.Save(rootDir + @"\Reklamer\lvlUp.jpg",
                    System.Drawing.Imaging.ImageFormat.Jpeg);
            }


        }
    }
}
