// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.IO;

namespace Broadcaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TaskbarJumpList tjl = new TaskbarJumpList(); 

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);                      
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Title = "Taskbar demo for AppId: " + TaskbarManager.Instance.ApplicationId;
            //tjl.AddTask();
        }

        private void schedule_Click(object sender, RoutedEventArgs e)
        {
            schedule_list.Visibility = Visibility.Visible;
        }

        private void dvd_Click(object sender, RoutedEventArgs e)
        {
            Import import = new Import();
            import.ShowDialog();
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {            
        }

        private void emergency_Click(object sender, RoutedEventArgs e)
        {
            startEmergency.Visibility = System.Windows.Visibility.Visible;
            cancelEmergency.Visibility = System.Windows.Visibility.Visible;
            emergency.Visibility = System.Windows.Visibility.Hidden;           
        }
        
        private void startEmergency_Click(object sender, RoutedEventArgs e)
        {
            stopEmergency.Visibility = System.Windows.Visibility.Visible;
            startEmergency.Visibility = System.Windows.Visibility.Hidden;
            cancelEmergency.Visibility = System.Windows.Visibility.Hidden;

            player.InEmergencyState = true;            
            player.mediaElement.Source = new Uri(Directory.GetCurrentDirectory() + "..\\Assets\\EmergencyMsg.jpg");
            player.SecondScreen.mediaElement1.Source = new Uri(Directory.GetCurrentDirectory() + "..\\Assets\\EmergencyMsg.jpg");
        }

        private void cancelEmergency_Click(object sender, RoutedEventArgs e)
        {
            startEmergency.Visibility = System.Windows.Visibility.Hidden;
            cancelEmergency.Visibility = System.Windows.Visibility.Hidden;
            emergency.Visibility = System.Windows.Visibility.Visible; 
        }

        private void stopEmergency_Click(object sender, RoutedEventArgs e)
        {
            stopEmergency.Visibility = System.Windows.Visibility.Hidden;
            emergency.Visibility = System.Windows.Visibility.Visible;

            player.InEmergencyState = false;
            player.mediaElement.Source = new Uri(Directory.GetCurrentDirectory() + "..\\Assets\\ComingUp.jpg");
            player.SecondScreen.mediaElement1.Source = new Uri(Directory.GetCurrentDirectory() + "..\\Assets\\ComingUp.jpg");
        }
    }
}