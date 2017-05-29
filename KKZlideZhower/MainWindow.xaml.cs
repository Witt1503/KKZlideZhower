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
using System.Net;
using TheArtOfDev.HtmlRenderer.WinForms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace KKZlideZhower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        private LinkedList<IViewer> pathList;
        private LinkedList<IViewer> reklamePathList;
        private LinkedListNode<IViewer> currentPath;
        private LinkedListNode<IViewer> currentReklamePath;
        private string rootDir;
        private DateTime lastLoaded;
        private LinkedListNode<IViewer> resume;
        private int spinner = 0;

        public MainWindow()
        {
            InitializeComponent();
            pathList = new LinkedList<IViewer>();
            rootDir = Settings.Default.AbsolutePath;

            createPathList(rootDir);
            currentPath = pathList.First;
            currentReklamePath = reklamePathList.First;
            isNewData();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Settings.Default.DisplayTimeMs);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private bool isNewData()
        {
            string rootDir2 = @"D:\Dropbox\Kælderen\KKZlideZhower";
            var last = File.GetLastWriteTime(rootDir2 + @"\..\Level-Up\LvlUp_F17.csv");
            if (last > lastLoaded)
            {
                lastLoaded = last;
                return true;
            }
            return false;

        }

        private void createPathList(string rootDir)
        {
            var list = new LinkedList<IViewer>();
            var reklameList = new LinkedList<IViewer>();
            List<string> allPaths = new List<string>();
            List<string> reklamePaths = new List<string>();
            try
            {
                foreach (var directory in Directory.GetDirectories(rootDir))
                {

                    var folder = directory.Split('\\');
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        
                        if (folder[folder.Length-1] == "Reklamer")
                        {
                            reklamePaths.Add(file);
                        }
                        else
                        {
                            allPaths.Add(file);
                        }
                    }
                }
                var random = new Random();

                while (allPaths.Count > 0)
                {
                    var randomNumber = random.Next(allPaths.Count);
                    var allPath = allPaths[randomNumber];
                    var tmp = new ImageViewer(allPath, this);
                    tmp.time = TimeSpan.FromMilliseconds(Settings.Default.DisplayTimeMs);
                    list.AddLast(tmp);
                    allPaths.RemoveAt(randomNumber);
                }
//                list.AddFirst(lvlUpData());
                while(reklamePaths.Count > 0)
                {
                    var randomNumber = random.Next(allPaths.Count);
                    var reklamePath = reklamePaths[randomNumber];
                    var tmp = new ImageViewer(reklamePath, this);
                    tmp.time = TimeSpan.FromMilliseconds(Settings.Default.DisplayTimeMs*1.5);

                    reklameList.AddLast(tmp);
                    reklamePaths.RemoveAt(randomNumber);
                }
            }
            catch
            (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            pathList = list;
            reklamePathList = reklameList;
        }

        private IViewer lvlUpData()
        {

            var lvlup = new HtmlViewer(this, rootDir);
            lvlup.time = TimeSpan.FromMilliseconds(Settings.Default.DisplayTimeMs);
            return lvlup;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            
            if (isNewData())
            {
                resume = currentPath.Next ?? currentPath.List.First;
                currentPath = currentPath.List.First;
            }
            else
            {
                if (spinner == Settings.Default.ReklameInterval)
                {
                    spinner = 0;
                    currentReklamePath = (resume) ?? (currentReklamePath.Next ?? currentReklamePath.List.First);
                    timer.Interval = currentReklamePath.Value.time;
                    currentReklamePath.Value.view();
                }
                else
                {
                    spinner++;
                    currentPath = (resume) ?? (currentPath.Next ?? currentPath.List.First);
                    timer.Interval = currentPath.Value.time;
                    currentPath.Value.view();
                }
                resume = null;
            }
            
        }
        -



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            currentPath = currentPath.List.First;
            currentPath.Value.view();
            timer.Start();
            this.KeyDown += new KeyEventHandler(MainWindow_Shutdown);
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