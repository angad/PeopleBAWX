using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Threading;
using System.Media;
using XMLReaderWriter;
using System.ComponentModel;

namespace Broadcaster
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        private SecondScreen secondScreen;
        private bool inEmergencyState = false;
        private DateTime comingUpNext = new DateTime();

        public Player()
        {
            InitializeComponent();            
            this.Loaded += Player_Loaded;
            secondScreen = new SecondScreen();
            secondScreen.Show();
            LoadScheduledVideo();
        }

        public SecondScreen SecondScreen
        {
            get { return secondScreen; }
            set { secondScreen = value; }
        }

        public bool InEmergencyState
        {
            get { return inEmergencyState; }
            set { inEmergencyState = value; }
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            mediaElement.MediaOpened += mediaElement_MediaOpened;            
        }       

        private void LoadScheduledVideo()
        {
            string path = Directory.GetCurrentDirectory() + "..\\Assets\\ComingUp.jpg";
            mediaElement.Source = new Uri(path);
            secondScreen.SetVideoSource(path);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();   
        }

        public DailyVideoFiles dvf = new DailyVideoFiles();
        public DateTime[] filesTime;
        public DateTime[] filesLength;
        public string[] filesPath;

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            /*
            UpdatePlayerUriDelegate update = new UpdatePlayerUriDelegate(UpdateNowPlaying);
            Dispatcher meDispatcher = mediaElement.Dispatcher;
            meDispatcher.BeginInvoke(update, "C:\\1.wmv");
            */
            
            int prevDay;
            string[] timeSplit;
            int[] timeInInt;
            int videoCount;
            bool isNewDay = true;

            UpdatePlayerUriDelegate updateNowPlaying = new UpdatePlayerUriDelegate(UpdateNowPlaying);
            Dispatcher mediaElementDispatcher = mediaElement.Dispatcher;

            UpdateCurrentTimeDelegate updateDisplayTime = new UpdateCurrentTimeDelegate(UpdateDisplayTime);
            Dispatcher displayTimeDispatcher = displayTime.Dispatcher;    

            do
            {
                //get videos ready
                prevDay = DateTime.Now.Day;
                string scheduleFileName = "C:\\PeopleBAWX\\" + Utilities.GetMonday(DateTime.Now) + ".xml";
                dvf = Utilities.GetTodayScheduleList(
                    scheduleFileName, Utilities.FormatDateYMD(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

                if (dvf == null)
                {
                    if (isNewDay)
                    {
                        MessageBox.Show("Nothing to broadcast today!");
                        videoCount = 0;
                        isNewDay = false;
                    }
                    displayTimeDispatcher.BeginInvoke(updateDisplayTime,
                            String.Format("{0:00}", DateTime.Now.Hour) + ":" + String.Format("{0:00}", DateTime.Now.Minute) + ":" + String.Format("{0:00}", DateTime.Now.Second));

                    Thread.Sleep(1000); //wait for 1 sec
                }
                else //do the broadcasting
                {
                    isNewDay = false;
                    videoCount = dvf.VideoFileList.Count;
                    filesTime = new DateTime[videoCount];
                    filesLength = new DateTime[videoCount];
                    filesPath = new string[videoCount];

                    for (int i = 0; i < videoCount; i++)
                    {
                        VideoFile vf = dvf.VideoFileList.ElementAt(i);
                        timeSplit = vf.Start_Time.Split(':');
                        timeInInt = new int[3];
                        for (int j = 0; j < 3; j++)
                        {
                            timeInInt[j] = Int32.Parse(timeSplit[j]);
                        }
                        filesTime[i] = new DateTime(
                            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                            timeInInt[0], timeInInt[1], timeInInt[2]);
                        filesPath[i] = vf.Path;

                        timeSplit = vf.Length.Split(':');
                        timeInInt = new int[3];
                        for (int j = 0; j < 3; j++)
                        {
                            timeInInt[j] = Int32.Parse(timeSplit[j]);
                        }
                        filesLength[i] = new DateTime(
                            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                            timeInInt[0], timeInInt[1], timeInInt[2]);
                    }                                    

                    int videoChecked = 0;

                    while (true)
                    {
                        if (comingUpNext.ToLongTimeString() == DateTime.Now.ToLongTimeString())
                        {
                            string path = Directory.GetCurrentDirectory() + "..\\Assets\\ComingUp.jpg";
                            mediaElementDispatcher.BeginInvoke(updateNowPlaying, path);
                        }

                        if (DateTime.Now.Day != prevDay) //next day reached
                        {
                            isNewDay = true;
                            break;
                        }
                        else if (videoChecked == videoCount)
                        {
                            //all the videos for today have been broadcasted
                            //wait for the next day
                        }
                        else
                        {
                            //the schedule for today came late
                            //some shows scheduled early need not be broadcast
                            if (filesTime[videoChecked].Hour < DateTime.Now.Hour ||
                                (filesTime[videoChecked].Hour == DateTime.Now.Hour &&
                                 filesTime[videoChecked].Minute < DateTime.Now.Minute) ||
                                (filesTime[videoChecked].Hour == DateTime.Now.Hour &&
                                 filesTime[videoChecked].Minute == DateTime.Now.Minute &&
                                 filesTime[videoChecked].Second < DateTime.Now.Second))
                            {
                                videoChecked++;
                                continue; //skip this iteration
                            }

                            //the time for broadcasting this particular video is reached
                            if (filesTime[videoChecked].ToLongTimeString() == DateTime.Now.ToLongTimeString())
                            {
                                mediaElementDispatcher.BeginInvoke(updateNowPlaying, "C:\\PeopleBAWX\\" + filesPath[videoChecked]);
                                comingUpNext = DateTime.Now.Add(new TimeSpan(
                                    filesLength[videoChecked].Hour, filesLength[videoChecked].Minute, filesLength[videoChecked].Second));
                                videoChecked++;
                            }                            
                        }
                        displayTimeDispatcher.BeginInvoke(updateDisplayTime,
                            String.Format("{0:00}", DateTime.Now.Hour) + ":" + String.Format("{0:00}", DateTime.Now.Minute) + ":" + String.Format("{0:00}", DateTime.Now.Second));

                        Thread.Sleep(1000); //wait for 1 sec
                    }
                }                
             
            } while (true);
            
        }

        public delegate void UpdatePlayerUriDelegate(string uri);

        public void UpdateNowPlaying(string uri)
        {
            if (!inEmergencyState)
            {
                mediaElement.Source = new Uri(uri); //loading is set to play
                secondScreen.SetVideoSource(uri);
            }
        }

        public delegate void UpdateCurrentTimeDelegate(string time);

        public void UpdateDisplayTime(string time)
        {
            displayTime.Text = time;
        }

        //The following method is part of the lab. Get the offset of the media element from the
        //main window and set the thumbnail clip to start at the offset and have the precise size
        //of the media element.
        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Vector offset = Utilities.GetOffset(Application.Current.MainWindow, (Visual)mediaElement);

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                IntPtr mainWindowHandle = new WindowInteropHelper(
                    Application.Current.MainWindow).Handle;
                System.Drawing.Rectangle clipRect = new System.Drawing.Rectangle(
                    (int)(offset.X - mediaElement.ActualWidth/2), (int)offset.Y,
                    (int)mediaElement.ActualWidth, (int)mediaElement.ActualHeight);

                TaskbarManager.Instance.TabbedThumbnail.SetThumbnailClip(mainWindowHandle, clipRect);
            }));
        }


        //ignore
        private void OnOpenMedia(object sender, RoutedEventArgs args)
        {
            CommonOpenFileDialog cfd = new CommonOpenFileDialog();
            CommonFileDialogFilter rtfFilter = new CommonFileDialogFilter("Windows media files", ".wmv");
            cfd.Filters.Add(rtfFilter);

            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
            {
                mediaElement.Source = new Uri(cfd.FileName, UriKind.RelativeOrAbsolute);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = (double)slider.Value;
        }
    }
}