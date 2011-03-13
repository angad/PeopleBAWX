using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLReaderWriter
{
    public class Schedule
    {
        public List<DailyVideoFiles> DailyVideoFilesList { get; set; }

        public Schedule()
        {
            DailyVideoFilesList = new List<DailyVideoFiles>();
        }

        public override string ToString()
        {
            string returnStr = "";
            for (int i = 0; i < DailyVideoFilesList.Count; i++)
            {
                returnStr += DailyVideoFilesList.ElementAt(i).ToString() + "\n";
            }
            return returnStr;
        }
    }

    public class FileHistory
    {
        public string Curr_Folder { get; set; } 
        public List<VideoFile> VideoFileList { get; set; }

        public FileHistory()
        {
            Curr_Folder = "";
            VideoFileList = new List<VideoFile>();
        }

        public override string ToString()
        {
            string returnStr = "Curr_Folder=" + Curr_Folder + ", \n";
            for (int i = 0; i < VideoFileList.Count; i++)
            {
                returnStr += "Video " + i + ": " + VideoFileList.ElementAt(i).ToString() + "\n";
            }
            return returnStr;
        }
    }

    public class DailyVideoFiles
    {
        public string Date { get; set; }
        public List<VideoFile> VideoFileList { get; set; }

        public DailyVideoFiles()
        {
            Date = "";
            VideoFileList = new List<VideoFile>();
        }

        public override string ToString()
        {
            string returnStr = "\tDate=" + Date + "\n";
            for (int i = 0; i < VideoFileList.Count; i++)
            {
                returnStr += "\tVideo " + i + ": " + VideoFileList.ElementAt(i).ToString() + "\n";
            }
            return returnStr;
        }
    }

    public class VideoFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Length { get; set; }
        public string Subtitles { get; set; }
        public string Start_Time { get; set; } //only for the file history
        public string End_Time { get; set; }
        public string Category { get; set; }//only for the file history
        public int Col { get; set; }
        public int Row { get; set; }
        public int Index { get; set; }


        public VideoFile()
        {
            Path = "";
            Name = "";
            Ext = "";
            Length = "";
            Subtitles = "";
            Start_Time = "";
            Category = "";
            Col = 0;
            Row = 0;
        }

        public override string ToString()
        {
            string returnStr = "Path=" + Path + ", Name=" + Name + "\n"
                + "Ext=" + Ext + ", Length=" + Length + ", Subtitles=" + Subtitles + ","
                + "Start_Time=" + Start_Time + ", Category=" + Category + Col + Row;
            return returnStr;
        }
    }
}
