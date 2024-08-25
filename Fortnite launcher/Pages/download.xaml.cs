﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp6.Pages
{
    public partial class download : UserControl
    {
        private DispatcherTimer timer;


        private void DownloadBuild_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "Your url for downloud";
            process.Start();
        }


        public download()
        {
            InitializeComponent();

            // Initialize and start the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Update every second
            timer.Tick += Timer_Tick;
            timer.Start();

        
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the clock content here
            clock.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}

