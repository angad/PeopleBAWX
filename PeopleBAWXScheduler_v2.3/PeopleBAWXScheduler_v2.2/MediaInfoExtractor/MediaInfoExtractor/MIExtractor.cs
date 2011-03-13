using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaInfoLib;

namespace MediaInfoExtractor
{
    public class MIExtractor
    {
        public static void Main()
        {
            //string[] result = ExtractInfo("1.wmv");
            //for (int i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine(result[i]);
            //}
        }

        public static string[] ExtractInfo(string path)
        {
            MediaInfo MI = new MediaInfo();            
            MI.Open(path); 
            string[] returnInfo = new string[5];

            //File name 0
            returnInfo[0] = MI.Get(0, 0, "FileName");

            //Size 1
            string sizeInKBStr = MI.Get(0, 0, "FileSize/String").Substring(
                0, MI.Get(0, 0, "FileSize/String").LastIndexOf(" ")).Replace(" ", "");
            double sizeInKB = Double.Parse(sizeInKBStr);
            double sizeInMB = (double)(sizeInKB / 1024);
            string sizeInMBStr = String.Format("{0:0.00}", sizeInMB); 
            returnInfo[1] = sizeInMBStr + " MB";

            //Date created 2
            returnInfo[2] = MI.Get(0, 0, "File_Created_Date").Substring(
                MI.Get(0, 0, "File_Created_Date").IndexOf(" ") + 1, MI.Get(0, 0, "File_Created_Date").LastIndexOf(".") - 4);

            //Performer 3
            returnInfo[3] = MI.Get(0, 0, "Performer");

            //Length 4
            returnInfo[4] = MI.Get(0, 0, "Duration/String3").Substring(0, MI.Get(0, 0, "Duration/String3").LastIndexOf("."));

            return returnInfo;
        }
    }
}
