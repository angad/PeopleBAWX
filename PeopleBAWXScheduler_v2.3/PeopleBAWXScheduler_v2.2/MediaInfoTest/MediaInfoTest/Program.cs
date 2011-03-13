using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaInfoLib;

namespace MediaInfoTest
{
    class Program
    {
        public static void Main()
        {
            MediaInfo MI = new MediaInfo();

            //change the files accordingly, tested with .avi, .mpg, .mpeg - these should be enough
            MI.Open("1.flv"); 

            string display = "";
            display += "FileName: " + MI.Get(0, 0, "FileName") + "\n";
            display += "FileExtension: " + MI.Get(0, 0, "FileExtension") + "\n";
            display += "Title: " + MI.Get(0, 0, "Title") + "\n";
            display += "Subtitle: " + MI.Get(0, 0, "Title/More") + "\n";
            display += "Tags: " + MI.Get(0, 0, "WM/Category") + "\n";
            display += "Comments: " + MI.Get(0, 0, "Comment") + "\n";

            string sizeInKBStr = MI.Get(0, 0, "FileSize/String").Substring(0, MI.Get(0, 0, "FileSize/String").LastIndexOf(" ")).Replace(" ", "");
            double sizeInKB = Double.Parse(sizeInKBStr);
            double sizeInMB = (double)(sizeInKB / 1024);
            string sizeInMBStr = String.Format("{0:0.00}", sizeInMB); 
            display += "Size: " + sizeInMBStr + " MB" + "\n";
            display += "Date created: " + MI.Get(0, 0, "File_Created_Date").Substring(MI.Get(0, 0, "File_Created_Date").IndexOf(" ") + 1, MI.Get(0, 0, "File_Created_Date").LastIndexOf(".")-4) + "\n";
            display += "Date modified: " + MI.Get(0, 0, "File_Modified_Date").Substring(MI.Get(0, 0, "File_Modified_Date").IndexOf(" ") + 1, MI.Get(0, 0, "File_Modified_Date").LastIndexOf(".")-4) + "\n";

            display += "\nVIDEO\n";
            display += "Length: " + MI.Get(0, 0, "Duration/String3").Substring(0, MI.Get(0, 0, "Duration/String3").LastIndexOf(".")) + "\n";
            display += "Frame width: " + MI.Get(StreamKind.Video, 0, "Width/String") + "\n";
            display += "Frame height: " + MI.Get(StreamKind.Video, 0, "Height/String") + "\n";
            if (MI.Get(StreamKind.Video, 0, "BitRate/String") != "")
                display += "Data rate: " + MI.Get(StreamKind.Video, 0, "BitRate/String").Substring(0, MI.Get(StreamKind.Video, 0, "BitRate/String").LastIndexOf(" ")).Replace(" ", "") + " kbps" + "\n";
            else
                display += "Data rate:\n";
            string frameRateOriStr = MI.Get(StreamKind.Video, 0, "FrameRate/String").Substring(0, MI.Get(StreamKind.Video, 0, "FrameRate/String").LastIndexOf(" ")).Replace(" ", "");
            double frameRate = Double.Parse(frameRateOriStr);
            string frameRateStr = String.Format("{0:0}", frameRate);
            display += "Frame rate: " + frameRateStr + " fps" + "\n";

            display += "\nAUDIO\n";
            string bitrateOriStr = MI.Get(StreamKind.Audio, 0, "BitRate/String").Substring(0, MI.Get(StreamKind.Audio, 0, "BitRate/String").LastIndexOf(" ")).Replace(" ", "");
            double bitrateRate = Double.Parse(bitrateOriStr);
            string bitrateRateStr = String.Format("{0:0}", bitrateRate);
            display += "Bitrate: " + bitrateRateStr + " kbps" + "\n";
            display += "Channels: " + MI.Get(StreamKind.Audio, 0, "Channel(s)/String") + "\n";
            string audioSampleRateOriStr = MI.Get(StreamKind.Audio, 0, "SamplingRate/String").Substring(0, MI.Get(StreamKind.Audio, 0, "SamplingRate/String").LastIndexOf(" ")).Replace(" ", "");
            double audioSampleRate = Double.Parse(audioSampleRateOriStr);
            string audioSampleRateStr = String.Format("{0:0}", audioSampleRate);
            display += "Audio sample rate: " + audioSampleRateStr + " kHz" + "\n";

            display += "\nMEDIA\n";
            display += "Artist: " + MI.Get(0, 0, "Performer") + "\n";
            display += "Year: " + MI.Get(0, 0, "Recorded_Date") + "\n";
            display += "Genre: " + MI.Get(0, 0, "Genre") + "\n";            

            MI.Close();

            Console.WriteLine(display);

        }
    
    }
}
