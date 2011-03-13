using System;
using System.Collections.Generic;
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
using XMLReaderWriter;

namespace Broadcaster
{
    /// <summary>
    /// Interaction logic for ScheduleList.xaml
    /// </summary>
    public partial class ScheduleList : UserControl
    {
        //TaskbarJumpList tjl = new TaskbarJumpList(); //jumplist

        public ScheduleList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ScheduleList_Loaded);
            displayDate.Text = Utilities.FormatDateDMY(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);            
            InitializeScheduleList();
        }

        private void InitializeScheduleList()
        {
            for (int j = 0; j < 48; j++)//generate row
            {
                AddVideoInfo(1, j);
            }       
        }

        private void ScheduleList_Loaded(object sender, RoutedEventArgs e)
        {
            //tjl.AddTask(); //for jumplist
            
            //Update the schedule list with the display date
            string scheduleFileName = "C:\\PeopleBAWX\\" + Utilities.GetMonday(DateTime.Now) + ".xml";
            DailyVideoFiles dvf = Utilities.GetTodayScheduleList(scheduleFileName,
                Utilities.FormatDateYMD(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            ResetScheduleList();
            if (dvf != null) UpdateScheduleList(dvf);
            
            //tjl.AddRecent(scheduleFileName);//for jumplist
        }

        private void AddVideoInfo(int col, int row)
        {
            TextBlock tb = new TextBlock();
            tb.Text = "";
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            tb.TextWrapping = TextWrapping.Wrap;
            if (row % 2 == 0)
                tb.Background = new SolidColorBrush(Colors.DarkGray);
            else 
                tb.Background = new SolidColorBrush(Colors.LightGray);
            scheduleList.Children.Add(tb);
        }

        private void closeSchedule_Click(object sender, RoutedEventArgs e)
        {
            calendar.Visibility = Visibility.Hidden;
            calendar.SelectedDate = DateTime.Now;
            displayDate.Text = Utilities.FormatDateDMY(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            this.Visibility = Visibility.Hidden;
        }

        private void showHideCalendar(object sender, MouseButtonEventArgs e)
        {
            if (calendar.Visibility.Equals(Visibility.Hidden))
                calendar.Visibility = Visibility.Visible;
            else calendar.Visibility = Visibility.Hidden;
        }

        
        List<VideoFile> videoList = new List<VideoFile>();

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //Update the schedule list with the display date
            DateTime date = calendar.SelectedDate.Value;
            displayDate.Text = Utilities.FormatDateDMY(date.Day, date.Month, date.Year);
            string scheduleFileName = "C:\\PeopleBAWX\\" + Utilities.GetMonday(date) + ".xml";
            DailyVideoFiles dvf = Utilities.GetTodayScheduleList(scheduleFileName, Utilities.FormatDateYMD(date.Year, date.Month, date.Day));
            ResetScheduleList();
            if(dvf!=null) UpdateScheduleList(dvf);

            //tjl.AddRecent(scheduleFileName); //for jumplist
        }

        private void ResetScheduleList()
        {
            for (int i = 24; i < scheduleList.Children.Count; i++)
            {
                TextBlock tb = (TextBlock)scheduleList.Children[i];
                tb.Text = "";
                tb.Tag = "";
                if (i % 2 == 0)
                    tb.Background = new SolidColorBrush(Colors.DarkGray);
                else
                    tb.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void UpdateScheduleList(DailyVideoFiles dvf)
        {
            /*
            MessageBox.Show(scheduleList.Children.Count + "");
            string list = "";
            for (int i = 0; i < scheduleList.Children.Count; i++)
            {
                 list += ("i=" + i + ": " + scheduleList.Children[i].ToString() + "\n");
                 if ((i+1) % 40 == 0)
                 {
                     MessageBox.Show(list);
                     list = "";
                 }
            }
            MessageBox.Show(list);
            */
            
            
            foreach (VideoFile vf in dvf.VideoFileList)
            {
                //MessageBox.Show(vf.Row + "");
                TextBlock tb = (TextBlock) scheduleList.Children[24 + vf.Row];
                tb.Text = vf.Path;
                tb.Background = Brushes.DeepPink;

                int noOfTextBlock = GetNoOfOccupiedTextBlock(vf.Length);
                //MessageBox.Show("noOfTextBlock:" + noOfTextBlock);
                if (noOfTextBlock > 1)
                {
                    Grid.SetRowSpan(tb, noOfTextBlock);
                    for (int i = 1; i < noOfTextBlock; i++)
                    {
                        TextBlock tb1 = (TextBlock)scheduleList.Children[24 + vf.Row + i];
                        tb1.Text = "";
                        tb1.Background = Brushes.Transparent;
                    }
                }
            }
             
        }

        private int GetNoOfOccupiedTextBlock(String length)
        {
            int hour = int.Parse(length.Substring(0, 2));
            int min = int.Parse(length.Substring(3, 2));
            int sec = int.Parse(length.Substring(6, 2));
            int num = hour * 2;
            num += (int)Math.Ceiling((Decimal)min / 30);
            if(num % 30 == 0 && sec > 0)
                num += 1;
            return num;
        }

        
        

    }
}
