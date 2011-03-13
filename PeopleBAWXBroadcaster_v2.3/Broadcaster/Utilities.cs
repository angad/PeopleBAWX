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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Diagnostics;
using RegistrationHelper;
using XMLReaderWriter;

namespace Broadcaster
{
    public static class Utilities
    {       

        #region Jumplist

        /*
        private static void InternalRegisterFileAssociations(
            bool unregister, string progId, bool registerInHKCU,
            string appId, string openWith, string[] extensions)
        {

            ProcessStartInfo psi = new ProcessStartInfo( 
                    typeof(RegistrationHelperMain).Assembly.Location);
            psi.Arguments =
                progId + " " +
                registerInHKCU + " "
                + appId
                + " \"" + openWith + "\" " +
                unregister + " "
                + string.Join(" ", extensions);
            psi.UseShellExecute = true;
            psi.Verb = "runas"; //Launch elevated
            Process.Start(psi).WaitForExit();
        }

        /// <summary>
        /// Registers file associations for an application.
        /// </summary>
        /// <param name="progId">The application's ProgID.</param>
        /// <param name="registerInHKCU">Whether to register the
        /// association per-user (in HKCU).  The only supported value
        /// at this time is <b>false</b>.</param>
        /// <param name="appId">The application's app-id.</param>
        /// <param name="openWith">The command and arguments to be used
        /// when opening a shortcut to a document.</param>
        /// <param name="extensions">The extensions to register.</param>
        public static void RegisterFileAssociations(string progId,
            bool registerInHKCU, string appId, string openWith,
            params string[] extensions)
        {
            InternalRegisterFileAssociations(
                false, progId, registerInHKCU, appId, openWith, extensions);
        }

        /// <summary>
        /// Unregisters file associations for an application.
        /// </summary>
        /// <param name="progId">The application's ProgID.</param>
        /// <param name="registerInHKCU">Whether to register the
        /// association per-user (in HKCU).  The only supported value
        /// at this time is <b>false</b>.</param>
        /// <param name="appId">The application's app-id.</param>
        /// <param name="openWith">The command and arguments to be used
        /// when opening a shortcut to a document.</param>
        /// <param name="extensions">The extensions to unregister.</param>
        public static void UnregisterFileAssociations(string progId,
            bool registerInHKCU, string appId, string openWith,
            params string[] extensions)
        {
            InternalRegisterFileAssociations(
                true, progId, registerInHKCU, appId, openWith, extensions);
        }

        public static bool IsApplicationRegistered(string appId)
        {
            try
            {
                using (RegistryKey progIdKey = Registry.ClassesRoot.OpenSubKey(appId))
                {
                    return progIdKey != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
            return false;
        }

        public static bool HasThumbnailPreview(UIElement element)
        {
            return TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(element) != null;
        }
        */
        
        #endregion

        #region Thumbnail Clip

        public static Vector GetOffset(Window parent, Visual visual)
        {
            GeneralTransform ge = visual.TransformToVisual(Application.Current.MainWindow);
            Point offset = ge.Transform(new Point(0, 0));
            return new Vector(offset.X, offset.Y);
        }

        #endregion

        #region Schedule

        public static DailyVideoFiles GetTodayScheduleList(string fileName, string date)
        {
            Schedule sc = XMLReader.ReadSchedule(fileName);
            List<DailyVideoFiles> list = sc.DailyVideoFilesList;

            foreach (DailyVideoFiles dvf in list)
            {
                if (dvf.Date == date)
                {
                    return dvf;
                }
            }

            return null;
        }

        public static string GetMonday(DateTime date)
        {
            string day = date.DayOfWeek.ToString();

            TimeSpan ts = new TimeSpan(1, 0, 0, 0);

            if (day.Equals("Tuesday"))
                date = date.Subtract(ts);
            if (day.Equals("Wednesday"))
                date = date.Subtract(ts).Subtract(ts);
            if (day.Equals("Thursday"))
                date = date.Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Friday"))
                date = date.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Saturday"))
                date = date.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Sunday"))
                date = date.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);

            string selected_monday = Utilities.FormatDateYMD(date.Year, date.Month, date.Day);
            return selected_monday;
        }

        public static string FormatDateYMD(int year, int month, int day)
        {
            string formattedDate = "";
            formattedDate += year.ToString() + "-";
            if (month.ToString().Length == 1)
                formattedDate += "0" + month.ToString() + "-";
            else
                formattedDate += month.ToString() + "-";
            if (day.ToString().Length == 1)
                formattedDate += "0" + day.ToString();
            else
                formattedDate += day.ToString();

            return formattedDate;
        }

        public static string FormatDateDMY(int day, int month, int year)
        {
            string formattedDate = "";


            if (day.ToString().Length == 1)
                formattedDate += "0" + day.ToString() + "-";
            else
                formattedDate += day.ToString() + "-";
            formattedDate += IntToMonth(month) + "-";
            formattedDate += year.ToString();

            return formattedDate;
        }

        public static string IntToMonth(int n)
        {
            string m = "";
            switch (n)
            {
                case 1: m = "Jan"; break;
                case 2: m = "Feb"; break;
                case 3: m = "Mar"; break;
                case 4: m = "Apr"; break;
                case 5: m = "May"; break;
                case 6: m = "Jun"; break;
                case 7: m = "Jul"; break;
                case 8: m = "Aug"; break;
                case 9: m = "Sep"; break;
                case 10: m = "Oct"; break;
                case 11: m = "Nov"; break;
                case 12: m = "Dec"; break;
            }
            return m;
        }
        public static int MonthToInt(string s)
        {
            if (s == "Jan")
                return 1;
            else if (s == "Feb")
                return 2;
            else if (s == "Mar")
                return 3;
            else if (s == "Apr")
                return 4;
            else if (s == "May")
                return 5;
            else if (s == "Jun")
                return 6;
            else if (s == "Jul")
                return 7;
            else if (s == "Aug")
                return 8;
            else if (s == "Sep")
                return 9;
            else if (s == "Oct")
                return 10;
            else if (s == "Nov")
                return 11;
            else if (s == "Dec")
                return 12;
            else return 0;
        }

        #endregion

    }
}