using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XMLReaderWriter
{
    public class XMLWriter
    {
        //Writes data to a schedule XML file
        public static void WriteSchedule(Schedule schedule, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "", "");
            dec.Encoding = "UTF-8";
            xmlDoc.AppendChild(dec);

            XmlElement body = xmlDoc.CreateElement("schedule");
            xmlDoc.AppendChild(body);

            foreach (DailyVideoFiles dailyVideoFiles in schedule.DailyVideoFilesList)
            {
                XmlElement day = xmlDoc.CreateElement("day");
                body.AppendChild(day);

                XmlElement date = xmlDoc.CreateElement("date");
                day.AppendChild(date);
                date.InnerText = dailyVideoFiles.Date;

                foreach (VideoFile videoFile in dailyVideoFiles.VideoFileList)
                {
                    XmlElement program = xmlDoc.CreateElement("program");
                    day.AppendChild(program);

                    XmlElement path = xmlDoc.CreateElement("path");
                    program.AppendChild(path);
                    path.InnerText = videoFile.Path;

                    XmlElement name = xmlDoc.CreateElement("name");
                    program.AppendChild(name);
                    name.InnerText = videoFile.Name;

                    XmlElement ext = xmlDoc.CreateElement("ext");
                    program.AppendChild(ext);
                    ext.InnerText = videoFile.Ext;

                    XmlElement subtitles = xmlDoc.CreateElement("subtitles");
                    program.AppendChild(subtitles);
                    if (videoFile.Subtitles != "")
                        subtitles.InnerText = videoFile.Subtitles;

                    XmlElement length = xmlDoc.CreateElement("length");
                    program.AppendChild(length);
                    length.InnerText = videoFile.Length;

                    XmlElement start_time = xmlDoc.CreateElement("start_time");
                    program.AppendChild(start_time);
                    start_time.InnerText = videoFile.Start_Time;
                    
                    XmlElement end_time = xmlDoc.CreateElement("end_time");
                    program.AppendChild(end_time);
                    end_time.InnerText = videoFile.End_Time;

                    XmlElement col = xmlDoc.CreateElement("col");
                    program.AppendChild(col);
                    col.InnerText = videoFile.Col.ToString();

                    XmlElement row = xmlDoc.CreateElement("row");
                    program.AppendChild(row);
                    row.InnerText = videoFile.Row.ToString();


                    XmlElement index = xmlDoc.CreateElement("index");
                    program.AppendChild(index);
                    index.InnerText = videoFile.Index.ToString();

                    XmlElement category = xmlDoc.CreateElement("category");
                    program.AppendChild(category);
                    if (videoFile.Category != "")
                        category.InnerText = videoFile.Category;
                }
            }

            xmlDoc.Save(filePath);
        }

        //Writes data to a file history XML file
        public static void WriteFileHistory(FileHistory fileHistory, string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "", "");
            dec.Encoding = "UTF-8";
            xmlDoc.AppendChild(dec);

            XmlElement body = xmlDoc.CreateElement("file_history");
            xmlDoc.AppendChild(body);

            XmlElement curr_folder = xmlDoc.CreateElement("curr_folder");
            body.AppendChild(curr_folder);
            curr_folder.InnerText = fileHistory.Curr_Folder;

            foreach (VideoFile videoFile in fileHistory.VideoFileList)
            {
                XmlElement program = xmlDoc.CreateElement("program");
                body.AppendChild(program);

                XmlElement path = xmlDoc.CreateElement("path");
                program.AppendChild(path);
                path.InnerText = videoFile.Path;

                XmlElement name = xmlDoc.CreateElement("name");
                program.AppendChild(name);
                name.InnerText = videoFile.Name;

                XmlElement ext = xmlDoc.CreateElement("ext");
                program.AppendChild(ext);
                ext.InnerText = videoFile.Ext;

                XmlElement subtitles = xmlDoc.CreateElement("subtitles");
                program.AppendChild(subtitles);
                if (videoFile.Subtitles != "")
                    subtitles.InnerText = videoFile.Subtitles;

                XmlElement length = xmlDoc.CreateElement("length");
                program.AppendChild(length);
                length.InnerText = videoFile.Length;
            }

            xmlDoc.Save(filePath);
        }
    }
}
