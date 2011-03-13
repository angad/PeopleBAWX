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
using System.IO;
using System.Collections.ObjectModel;
using SimplestDragDrop;
using System.Windows.Media.Animation;
using XMLReaderWriter;

namespace Scheduler
{
    class AdsFiller
    {
        string schedule_file;

        DailyVideoFiles[] dailyList = new DailyVideoFiles[7];
        VideoFile[] videoList;
        DailyVideoFiles[] dailyList_with_ads = new DailyVideoFiles[7];
        VideoFile[] videoList_with_ads;

        public void load_schedule(string f)
        {
            videoList = new VideoFile[1024];

            for (int i = 0; i < 7; i++)
            {
                dailyList[i] = new DailyVideoFiles();
            }
            Schedule s = new Schedule();
            s = XMLReader.ReadSchedule(f);
            List<DailyVideoFiles> list = s.DailyVideoFilesList;

            list.CopyTo(dailyList, 0);

            foreach (DailyVideoFiles dailyFile in list)
            {
                //MessageBox.Show("how many days?" + dailyFile.VideoFileList.Count);
                foreach (VideoFile v in dailyFile.VideoFileList)
                {
                    int index = v.Index;
                    videoList[index] = v;
                }
            }

            get_diff(dailyList);

        }


        public void get_diff(DailyVideoFiles[] list)
        {
            String t1 = "", t2 = "";
            int flag = 1;
            //VideoFile v1;
            VideoFile v2 = null;

            foreach (DailyVideoFiles dailyFile in list)
            {
                foreach (VideoFile v in dailyFile.VideoFileList)
                {
                    int index = v.Index;
                    t1 = v.End_Time;

                    foreach (VideoFile v1 in dailyFile.VideoFileList)
                    {
                        if (v1.Index > index)
                        {
                            flag = 0;
                            t2 = v1.Start_Time;
                            v2 = v1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        flag = 1;
                        String[] time1 = t1.Split(':');
                        String[] time2 = t2.Split(':');
                        TimeSpan ts1 = new TimeSpan(int.Parse(time1[0]), int.Parse(time1[1]), int.Parse(time1[2]));
                        TimeSpan ts2 = new TimeSpan(int.Parse(time2[0]), int.Parse(time2[1]), int.Parse(time2[2]));

                        fill_ads(ts1, ts2, v, v2, list);
                    }
                }
            }
        }


        private void fill_ads(TimeSpan ts1, TimeSpan ts2, VideoFile v1, VideoFile v2, DailyVideoFiles[] list)
        {
            TimeSpan diff = ts2.Subtract(ts1);

            FileHistory fileHistory = new FileHistory();
            fileHistory = XMLReader.ReadFileHistory("ad_log.xml");
            VideoFile[] vf = new VideoFile[1024];

            TimeSpan[] lengths = new TimeSpan[vf.Count()];
            fileHistory.VideoFileList.CopyTo(vf);

            TimeSpan max = lengths[0];

            VideoFile ad = new VideoFile();

            for (int i = 0; vf[i] != null; i++)
            {
                //MessageBox.Show(vf[i].Col.ToString());
                String[] length = vf[i].Length.Split(':');
                lengths[i] = new TimeSpan(int.Parse(length[0]), int.Parse(length[1]), int.Parse(length[2]));
                if (lengths[i] > max && lengths[i] < diff)
                {
                    max = lengths[i];
                    ad = vf[i];
                }
            }
            MessageBox.Show(max.ToString());
        }


        String[] date_gen()
        {
            String[] str = new String[7];
            String[] date = schedule_file.Split('-');

            int day = int.Parse(date[0]);

            int mon = month_to_int(date[1]);

            int year = int.Parse(date[2]);

            DateTime dt = new DateTime(year, mon, day);

            for (int i = 0; i < 7; i++)
            {
                str[i] = dt.Day + "-" + to_month(dt.Month) + "-" + dt.Year;
                dt = dt.AddDays(1);
            }
            return str;
        }

        int month_to_int(string s)
        {
            if (s == "January")
                return 1;
            else if (s == "February")
                return 2;
            else if (s == "March")
                return 3;
            else if (s == "April")
                return 4;
            else if (s == "May")
                return 5;
            else if (s == "June")
                return 6;
            else if (s == "July")
                return 7;
            else if (s == "August")
                return 8;
            else if (s == "September")
                return 9;
            else if (s == "October")
                return 10;
            else if (s == "November")
                return 11;
            else if (s == "December")
                return 12;
            else return 0;
        }

        private string to_month(int n)
        {
            string m = "";
            switch (n)
            {
                case 1: m = "January"; break;
                case 2: m = "February"; break;
                case 3: m = "March"; break;
                case 4: m = "April"; break;
                case 5: m = "May"; break;
                case 6: m = "June"; break;
                case 7: m = "July"; break;
                case 8: m = "August"; break;
                case 9: m = "September"; break;
                case 10: m = "October"; break;
                case 11: m = "November"; break;
                case 12: m = "December"; break;
            }
            return m;
        }


        Schedule sc = new Schedule();

        public void save_weekSchedule_with_ads()
        {
            sc.DailyVideoFilesList = new List<DailyVideoFiles>();
            String[] dates = new String[7];

            videoList_with_ads = videoList;
            dailyList_with_ads = dailyList;

            dates = date_gen();
            
            for (int i = 0; i < 7; i++)
            {
                dailyList[i] = new DailyVideoFiles();
            }

            foreach (VideoFile video in videoList)
            {
                if (video != null)
                {
                    int day = video.Col - 1;
                    MessageBox.Show(day.ToString());
                    dailyList[day].Date = dates[day];
                    dailyList[day].VideoFileList.Add(video);
                }
            }

            foreach (VideoFile video in videoList_with_ads)
            {
                if (video != null)
                {
                    int day = video.Col - 1;
                    dailyList_with_ads[day].Date = dates[day];
                    dailyList_with_ads[day].VideoFileList.Add(video);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                sc.DailyVideoFilesList.Add(dailyList[i]);
                sc.DailyVideoFilesList.Add(dailyList_with_ads[i]);
            }

            

            string file = schedule_file + "_ads" + ".xml";
            XMLWriter.WriteSchedule(sc, file);
        }
    }
}
