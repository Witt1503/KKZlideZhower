using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using KKZlideZhower.Properties;

namespace KKZlideZhower
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    DispatcherTimer timer;
    private LinkedList<string> pathList;
    private LinkedListNode<string> currentPath;
    private string rootDir;

    public MainWindow()
    {
      InitializeComponent();
      pathList = new LinkedList<string>();
      rootDir = Settings.Default.AbsolutePath;
      

      pathList = createPathList(rootDir);
      currentPath = pathList.First;
      timer = new DispatcherTimer();
      timer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Default.DisplayTimeMs);
      timer.Tick += new EventHandler(timer_Tick);
    }

    private LinkedList<string> createPathList(string rootDir)
    {
      LinkedList<string> list = new LinkedList<string>();
      List<string> allPaths = new List<string>();
      try
      {
        foreach (var directory in Directory.GetDirectories(rootDir))
        {
          foreach (var file in Directory.GetFiles(directory))
          {
            allPaths.Add(file);
          }
        }
        var random = new Random();
        
        while (allPaths.Count > 0)
        {
          var randomNumber = random.Next(allPaths.Count);
          var allPath = allPaths[randomNumber];
          
          list.AddLast(allPath);
          allPaths.RemoveAt(randomNumber);
        }
      }
      catch
      (Exception e)
      {

        Console.WriteLine(e.Message);
      }
      return list;
    }

    void timer_Tick(object sender, EventArgs e)
    {
      currentPath = currentPath.Next ?? currentPath.List.First;
      PlaySlideShow(currentPath.Value);
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      currentPath = currentPath.List.First;
      PlaySlideShow(currentPath.Value);
      this.KeyDown += new KeyEventHandler(MainWindow_Shutdown);
    }
    private void PlaySlideShow(string path)
    {
      BitmapImage image = new BitmapImage();
      image.BeginInit();
      image.UriSource = new Uri(path, UriKind.Absolute);
      image.EndInit();
      myImage.Source = image;
      myImage.Stretch = Stretch.Fill;
      myImage.StretchDirection = StretchDirection.Both;
      timer.IsEnabled = true;
      var txt = path.Split('\\');
      Overlay.Text = txt[txt.Length-2] == "Reklamer" ? "" : txt[txt.Length - 2];
      //progressBar1.Value = ctr;
    }
    void MainWindow_Shutdown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        Application.Current.Shutdown();
      }
    }
  }
}