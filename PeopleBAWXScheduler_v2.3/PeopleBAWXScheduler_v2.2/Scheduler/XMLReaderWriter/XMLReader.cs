using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace XMLReaderWriter
{
    public class XMLReader
    {
        //Reads data from a schedule XML file   
        public static Schedule ReadSchedule(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Schedule s = new Schedule();
                for (int i = 0; i < 7; i++)
                {
                    DailyVideoFiles d = new DailyVideoFiles();
                    s.DailyVideoFilesList.Add(d);
                }
                XMLWriter.WriteSchedule(s, filePath);
            }

            Schedule schedule = new Schedule();
            List<DailyVideoFiles> dailyVideoFilesList = new List<DailyVideoFiles>();


            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filePath);
            XmlNodeList xmlNodeList_Day = xmldoc.ChildNodes;
            XmlNodeList xmlNodeList_Program;
            XmlNode xmlNode_Day = xmlNodeList_Day[1];
            XmlNode xmlNode_Program;
            if (xmlNode_Day.Name == "schedule")
            {
                xmlNodeList_Day = xmlNode_Day.ChildNodes;
                DailyVideoFiles dailyVideoFiles = new DailyVideoFiles();
                for (int i = 0; i < 7; i++)
                {
                    dailyVideoFiles = new DailyVideoFiles();
                    xmlNode_Day = xmlNodeList_Day[i];

                    if (xmlNode_Day.Name == "day")
                    {
                        //System.Windows.MessageBox.Show("day");
                        List<VideoFile> videoFileList = new List<VideoFile>();
                        xmlNodeList_Program = xmlNode_Day.ChildNodes;
                        for (int j = 0; j < xmlNodeList_Program.Count; j++)
                        {
                            //videoFileList = new List<VideoFile>();
                            if (j == 0)
                            {
                                xmlNode_Program = xmlNodeList_Program[0];
                                if (xmlNode_Program.Name == "date")
                                    dailyVideoFiles.Date = xmlNode_Program.InnerText;
                                else throw new XMLFileFormatException("missing \"date\" element");
                            }
                            else
                            {
                                xmlNode_Program = xmlNodeList_Program[j];
                                if (xmlNode_Program.Name == "program")
                                {
                                    //System.Windows.MessageBox.Show("program");
                                    XmlNodeList xmlNodeList1 = xmlNode_Program.ChildNodes;
                                    VideoFile videoFile = new VideoFile();
                                    foreach (XmlNode xmlNode1 in xmlNodeList1)
                                    {
                                        if (xmlNode1.Name == "path")
                                            videoFile.Path = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "name")
                                            videoFile.Name = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "ext")
                                            videoFile.Ext = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "length")
                                            videoFile.Length = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "subtitles")
                                            videoFile.Subtitles = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "start_time")
                                            videoFile.Start_Time = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "end_time")
                                            videoFile.End_Time = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "category")
                                            videoFile.Category = xmlNode1.InnerXml;
                                        else if (xmlNode1.Name == "col")
                                            videoFile.Col = int.Parse(xmlNode1.InnerXml);
                                        else if (xmlNode1.Name == "row")
                                            videoFile.Row = int.Parse(xmlNode1.InnerXml);
                                        else if (xmlNode1.Name == "index")
                                            videoFile.Index = int.Parse(xmlNode1.InnerXml);
                                        else throw new XMLFileFormatException("unknown \"" + xmlNode1.Name + "\" element");
                                    }
                                    videoFileList.Add(videoFile);
                                }
                                else
                                {
                                    throw new XMLFileFormatException("missing \"program\" element");
                                }
                            }
                        } //end reading programs in a day
                        dailyVideoFiles.VideoFileList = videoFileList;
                    }
                    else
                    {
                        throw new XMLFileFormatException("missing \"day\" element");
                    }
                    //end reading every day's programs in a week

                    if (dailyVideoFiles.VideoFileList.Count != 0)
                    {
                        schedule.DailyVideoFilesList.Add(dailyVideoFiles);
                    }
                }
            }
            else
                throw new XMLFileFormatException("missing \"schedule\" element");

            return schedule;
        }

        //Reads data from a file history XML file
        public static FileHistory ReadFileHistory(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FileHistory fh = new FileHistory();
                XMLWriter.WriteFileHistory(fh, filePath);
            }

            FileHistory fileHistory = new FileHistory();
            List<VideoFile> videoFileList = new List<VideoFile>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filePath);
            XmlNodeList xmlNodeList = xmldoc.ChildNodes;
            XmlNode xmlNode = xmlNodeList[1];
            if (xmlNode.Name == "file_history")
            {
                xmlNodeList = xmlNode.ChildNodes;

                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    if (i == 0)
                    {
                        xmlNode = xmlNodeList[0];
                        if (xmlNode.Name == "curr_folder")
                            fileHistory.Curr_Folder = xmlNode.InnerText;
                        else throw new XMLFileFormatException("missing \"curr_folder\" element");
                    }
                    else
                    {
                        xmlNode = xmlNodeList[i];
                        if (xmlNode.Name == "program")
                        {
                            XmlNodeList xmlNodeList1 = xmlNode.ChildNodes;
                            VideoFile videoFile = new VideoFile();
                            foreach (XmlNode xmlNode1 in xmlNodeList1)
                            {
                                if (xmlNode1.Name == "path")
                                    videoFile.Path = xmlNode1.InnerXml;
                                else if (xmlNode1.Name == "name")
                                    videoFile.Name = xmlNode1.InnerXml;
                                else if (xmlNode1.Name == "ext")
                                    videoFile.Ext = xmlNode1.InnerXml;
                                else if (xmlNode1.Name == "length")
                                    videoFile.Length = xmlNode1.InnerXml;
                                else if (xmlNode1.Name == "subtitles")
                                    videoFile.Subtitles = xmlNode1.InnerXml;
                                else throw new XMLFileFormatException("unknown \"" + xmlNode1.Name + "\" element");
                            }
                            videoFileList.Add(videoFile);
                        }
                        else
                        {
                            throw new XMLFileFormatException("missing \"row\" element");
                        }
                    }

                }
                fileHistory.VideoFileList = videoFileList;

            }
            else throw new XMLFileFormatException("missing \"file_history\" element");
            return fileHistory;
        }

    }
}
