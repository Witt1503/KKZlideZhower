﻿using System;
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
        private LinkedListNode<IViewer> currentPath;
        private string rootDir;
        private DateTime lastLoaded;
        private LinkedListNode<IViewer> resume;

        public MainWindow()
        {
            InitializeComponent();
            pathList = new LinkedList<IViewer>();
            rootDir = Settings.Default.AbsolutePath;

            pathList = createPathList(rootDir);
            currentPath = pathList.First;
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

        private LinkedList<IViewer> createPathList(string rootDir)
        {
            var list = new LinkedList<IViewer>();
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
                    var tmp = new ImageViewer(allPath, this);
                    if (tmp.isReklame)
                    {
                        tmp.time = TimeSpan.FromMilliseconds(1.5 * Settings.Default.DisplayTimeMs);
                    }
                    else
                    {
                        tmp.time = TimeSpan.FromMilliseconds(Settings.Default.DisplayTimeMs);

                    }
                    list.AddLast(tmp);
                    allPaths.RemoveAt(randomNumber);
                }
                list.AddFirst(lvlUpData());
            }
            catch
            (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            return list;
        }

        private IViewer lvlUpData()
        {

            var lvlup = new HtmlViewer(this, rootDir);
            lvlup.time = TimeSpan.FromMilliseconds(Settings.Default.DisplayTimeMs);
            return lvlup;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // Add something that loads the lvl-up score, if it has been updated.
            if (isNewData())
            {
                resume = currentPath.Next ?? currentPath.List.First;
                currentPath = currentPath.List.First;
            }
            else
            {

                currentPath = (resume)??(currentPath.Next ?? currentPath.List.First);
                resume = null;
            }
            timer.Interval = currentPath.Value.time;
            currentPath.Value.view();
        }




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