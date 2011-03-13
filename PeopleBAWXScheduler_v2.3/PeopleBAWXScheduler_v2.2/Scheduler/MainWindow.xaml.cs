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
using MediaInfoLib;
using MediaInfoExtractor;

namespace Scheduler
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>


    public partial class UI : Window
    {

        #region Initial
        DragDropHandler ddh = new DragDropHandler();
        AdsFiller af = new AdsFiller();
        Canvas popup_canvas = new Canvas();
        TextBlock infoPop = new TextBlock();
        TextBlock infoBug = new TextBlock();
        Grid gPop = new Grid();
        LinearGradientBrush brush = new LinearGradientBrush();
        //MIExtractor MIE = new MIExtractor();

        public UI() //Constructor
        {
            InitializeComponent();

            generate_timetable1();
            generate_timetable2();

            read_filehistory();
            read_adhistory();

            Initialize_media();
            Initialize_Calendar();
            Initialize_Schedule();
            Initialize_Popup();
            Initialize_brush();

        }
        private void Initialize_media()
        {
            mediaElement1.Volume = 0.5;
            slider1.Value = mediaElement1.Volume * 10;
            slider1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider1_ValueChanged);
            search_box.KeyUp += new KeyEventHandler(search_box_KeyUp);
            search_box.Focus();
            search_box.CaretBrush = Brushes.Transparent;
        }
        private void Initialize_Calendar()
        {
            date_init();
            calendar_week_generate();
            calendar_day_generate();
        }
        private void Initialize_Schedule()
        {
            s_init();
            s_loadWeek(get_DailyScheduleList("C:\\PeopleBAWX\\" + schedule_file + ".xml"));
            calendar_day.FirstDayOfWeek = DayOfWeek.Monday;
        }
        private void Initialize_Popup() {
            main_canvas.Children.Add(popup_canvas);
            popup_canvas.Children.Add(infoPop);
            popup_canvas.Children.Add(gPop);
            //popup_canvas.Children.Add(infoBug);
            
            infoPop.Background = Brushes.Pink;
            infoBug.Background = Brushes.Green;
            gPop.Height = 30;
            gPop.Width = 60;
            gPop.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            gPop.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            gPop.Visibility = System.Windows.Visibility.Hidden;
            gPop.Background = new LinearGradientBrush(Colors.LightGray, Colors.Gray, 90.0);
        }
        private void Initialize_brush() 
        {
            brush.StartPoint = new Point(0.5, 1);
            brush.EndPoint = new Point(0.5, 0);
            brush.GradientStops.Add(new GradientStop(Colors.Pink, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.DeepPink, 0.25));
        }
        #endregion

        #region FILE HANDLER
        /*------------------------------------File Handler---------------------------------------*/
        private object dummyNode = null;
        public string SelectedImagePath { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            Table list = new Table();
            TableRowGroup rg = new TableRowGroup();
            list.RowGroups.Add(rg);

            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {

                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                    foreach (string s in Directory.GetFiles(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        string a = s.Substring(s.LastIndexOf('.') + 1);
                        if (a.Equals("wmv") || a.Equals("avi") || a.Equals("flv") || a.Equals("mpg") || a.Equals("jpg") || a.Equals("mpeg"))
                        {
                            subitem.Tag = s;
                            subitem.FontWeight = FontWeights.Normal;
                            subitem.Items.Add(dummyNode);
                            subitem.Expanded += new RoutedEventHandler(folder_Expanded);

                            //Mouse Over Event Listeners
                            subitem.MouseEnter += new MouseEventHandler(vid_preview);
                            subitem.MouseLeave += new MouseEventHandler(vid_preview_pause);

                            subitem.Foreground = new SolidColorBrush(Colors.Green);

                            //Double Click to load file in File List
                            subitem.MouseDoubleClick += new MouseButtonEventHandler(datagrid_subitem_doubleClick);


                            //Drag event listeners
                            subitem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.subitem_PreviewMouseLeftButtonDown);
                            subitem.PreviewMouseMove += new MouseEventHandler(ddh.subitem_PreviewMouseMove);

                            item.Items.Add(subitem);
                            int sub_index = item.Items.IndexOf(subitem);

                            //Right Click Menu
                            ContextMenu m = new System.Windows.Controls.ContextMenu();

                            //Delete
                            MenuItem add = new MenuItem();
                            add.Header = "Import";
                            add.Click += new RoutedEventHandler(add_click);
                            add.Tag = subitem.Tag.ToString() + "#" + sub_index;
                            m.Items.Add(add);

                            MenuItem prop = new MenuItem();
                            prop.Header = "Properties";
                            prop.Click += new RoutedEventHandler(prop_Click);
                            prop.Tag = subitem.Tag.ToString();
                            m.Items.Add(prop);

                            subitem.ContextMenu = m;
                        }
                    }
                }
                catch (Exception) { }
            }
        }


        void prop_Click(object sender, RoutedEventArgs e)
        {
            MenuItem prop = (MenuItem)sender;

            string text;
            string[] info = MIExtractor.ExtractInfo(prop.Tag.ToString());

            text = "Title:     " + info[0] + "\n";
            text += "Size:     " + info[1] + "\n";
            text += "Length: " + info[4] + "\n";
            text += "Performer: " + info[3] + "\n";
            text += "Date Created: " + info[2];

            MessageBox.Show(text);
        }

        void add_click(object sender, RoutedEventArgs e)
        {
            MenuItem sub = (MenuItem)sender;

            mediaElement1.Source = new Uri(sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#')));

            Uri src = get_img_src(sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#')));

            File_List fl = (new File_List()
            {
                Name = sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#')).Substring(sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#') - 1).LastIndexOf("\\") + 1),
                path = sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#')),
                len = mediaElement1.NaturalDuration.ToString(),
                img = new BitmapImage(src),
            });

            string dgr_tag = sub.Tag.ToString().Substring(0, sub.Tag.ToString().LastIndexOf('#')) + "#" + mediaElement1.NaturalDuration.ToString();

            init_dgr(fl, dgr_tag);
        }


        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }
        }

        //Grid popup2 = new Grid();
        //mouseover event for preview video in the file tree
        private void vid_preview(object sender, MouseEventArgs e)
        {
            TreeViewItem sub = (TreeViewItem)sender;
            sub.Foreground = new SolidColorBrush(Colors.Bisque);
            mediaElement1.Source = new Uri(sub.Tag.ToString());

            mediaElement1.Play();
        }

        private void vid_preview_pause(object sender, MouseEventArgs e)
        {
            TreeViewItem sub = (TreeViewItem)sender;
            sub.Foreground = new SolidColorBrush(Colors.Green);
            mediaElement1.Pause();
        }

        /*-------------------------File Handler Ends---------------------------*/
        #endregion

        #region FILE HISTORY
        /*--------------------------File History List----------------------------------*/

        ObservableCollection<File_List> svfl = new ObservableCollection<File_List>();
        ObservableCollection<File_List> safl = new ObservableCollection<File_List>();

        private void datagrid_subitem_doubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem sub = (TreeViewItem)sender;
            mediaElement1.Source = new Uri(sub.Tag.ToString());

            Uri src = get_img_src(sub.Tag.ToString());

            File_List fl = (new File_List()
            {
                Name = sub.Tag.ToString().Substring(sub.Tag.ToString().LastIndexOf("\\") + 1),
                path = sub.Tag.ToString(),
                len = mediaElement1.NaturalDuration.ToString().Substring(0, 8),
                img = new BitmapImage(src),
            });

            string dgr_tag = sub.Tag.ToString() + "#" + mediaElement1.NaturalDuration.ToString().Substring(0, 8);

            init_dgr(fl, dgr_tag);
        }

        void dataGrid1_Drop(object sender, DragEventArgs e)
        {
            DataGrid dg_drop = (DataGrid)sender;

            Uri src = get_img_src(e.Data.GetData("Text").ToString());

            //to prevent multiple copies of rows being created
            if (e.Data.GetData("Text").ToString().Contains("#"))
                return;

            File_List fl = (new File_List()
            {
                Name = e.Data.GetData("Text").ToString().Substring(e.Data.GetData("Text").ToString().LastIndexOf("\\") + 1),
                img = new BitmapImage(src),
                len = mediaElement1.NaturalDuration.ToString().Substring(0, 8),
                path = e.Data.GetData("Text").ToString()
            });

            string dgr_tag = e.Data.GetData("Text").ToString() + "#" + mediaElement1.NaturalDuration.ToString().Substring(0, 8);
            init_dgr(fl, dgr_tag);
        }

        //A function to initialise a data grid row in the datagrid1 and add event listeners
        FileHistory fh = new FileHistory();
        void init_dgr(File_List fl, string dgr_tag)
        {

            foreach (File_List f in svfl)
            {
                if (f.path == fl.path)
                {
                    return;
                }
            }
            if (fl.len.Equals("Automati")) return;
           
            DataGridRow dgr = new DataGridRow();
            dgr.Tag = dgr_tag;
            dgr.Background = Brushes.Gray;
            dgr.Item = fl;


            //Drag event listeners

            if (File.Exists(fl.path))
            {
                dgr.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.dataGrid1_PreviewMouseLeftButtonDown);
                dgr.PreviewMouseMove += new MouseEventHandler(ddh.dataGrid1_PreviewMouseMove);
            }
            else dgr.ToolTip = "File Does not exist!";

            //Mouse Over Event Listeners
            dgr.MouseEnter += new MouseEventHandler(dgr_MouseEnter);
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave);

            dgr.MouseDoubleClick += new MouseButtonEventHandler(dgr_MouseDoubleClick);
            dataGrid1.Items.Add(dgr);
            int dgr_index = dataGrid1.Items.IndexOf(dgr);

            //Right Click Menu
            ContextMenu m = new System.Windows.Controls.ContextMenu();

            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(del_click);
            del.Tag = "delete#" + dgr_index;
            m.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(prop_Click);
            prop.Tag = fl.path;
            m.Items.Add(prop);

            //Edit File Name

            dgr.ContextMenu = m;

            VideoFile vfl = new VideoFile();
            vfl.Path = fl.path;
            vfl.Name = fl.Name;
            vfl.Length = fl.len;
            vfl.Ext = fl.path.Substring(fl.path.LastIndexOf('.') + 1);

            fh.VideoFileList.Add(vfl);
            XMLWriter.WriteFileHistory(fh, "C:\\PeopleBAWX\\fh_log.xml");

            svfl.Add(fl);

        }


        void init_dgr_search(File_List fl, string dgr_tag)
        {
            if (fl.len.Equals("Automati")) return;

            DataGridRow dgr = new DataGridRow();
            dgr.Tag = dgr_tag;

            dgr.Item = fl;

            //Drag event listeners

            if (File.Exists(fl.path))
            {
                dgr.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.dataGrid1_PreviewMouseLeftButtonDown);
                dgr.PreviewMouseMove += new MouseEventHandler(ddh.dataGrid1_PreviewMouseMove);
            }
            else dgr.ToolTip = "File Does not exist!";

            //Mouse Over Event Listeners
            dgr.MouseEnter += new MouseEventHandler(dgr_MouseEnter);
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave);

            dgr.MouseDoubleClick += new MouseButtonEventHandler(dgr_MouseDoubleClick);
            dataGrid1.Items.Add(dgr);
            int dgr_index = dataGrid1.Items.IndexOf(dgr);

            //Right Click Menu
            ContextMenu m = new System.Windows.Controls.ContextMenu();

            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(del_click);
            del.Tag = "delete#" + dgr_index;
            m.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(prop_Click);
            prop.Tag = fl.path;
            m.Items.Add(prop);

            //Edit File Name

            dgr.ContextMenu = m;

            fl.idx = dgr_index;
            //svfl.Add(fl);
        }
        //dataGrid2
        private void datagrid2_subitem_doubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem sub = (TreeViewItem)sender;
            mediaElement1.Source = new Uri(sub.Tag.ToString());

            Uri src = get_img_src(sub.Tag.ToString());

            File_List fl = (new File_List()
            {
                Name = sub.Tag.ToString().Substring(sub.Tag.ToString().LastIndexOf("\\") + 1),
                path = sub.Tag.ToString(),
                len = mediaElement1.NaturalDuration.ToString().Substring(0, 8),
                img = new BitmapImage(src),
            });

            string dgr_tag = sub.Tag.ToString() + "#" + mediaElement1.NaturalDuration.ToString().Substring(0, 8);

            init_dgr2(fl, dgr_tag);
        }

        void dataGrid2_Drop(object sender, DragEventArgs e)
        {
            DataGrid dg_drop = (DataGrid)sender;
            Uri src = get_img_src(e.Data.GetData("Text").ToString());

            //to prevent multiple copies of rows being created
            if (e.Data.GetData("Text").ToString().Contains("#"))
                return;

            File_List fl = (new File_List()
            {
                Name = e.Data.GetData("Text").ToString().Substring(e.Data.GetData("Text").ToString().LastIndexOf("\\") + 1),
                img = new BitmapImage(src),
                len = mediaElement1.NaturalDuration.ToString().Substring(0, 8),
                path = e.Data.GetData("Text").ToString()
            });

            string dgr_tag = e.Data.GetData("Text").ToString() + "#" + mediaElement1.NaturalDuration.ToString().Substring(0, 8);
            init_dgr2(fl, dgr_tag);
        }


        //A function to initialise a data grid row in the datagrid1 and add event listeners
        FileHistory fh2 = new FileHistory();
        void init_dgr2(File_List fl, string dgr_tag)
        {
            foreach (File_List f in safl)
            {
                if (f.path == fl.path)
                    return;
            }

            if (fl.len.Equals("Automati")) return;

            
            DataGridRow dgr = new DataGridRow();
            dgr.Tag = dgr_tag;
            dgr.Background = Brushes.Gray;
            dgr.Item = fl;

            if (File.Exists(fl.path))
            {
                dgr.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.dataGrid1_PreviewMouseLeftButtonDown);
                dgr.PreviewMouseMove += new MouseEventHandler(ddh.dataGrid1_PreviewMouseMove);
            }
            else dgr.ToolTip = "File Does not exist!";

            //Mouse Over Event Listeners
            dgr.MouseEnter += new MouseEventHandler(dgr_MouseEnter2);
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave);

            dgr.MouseDoubleClick += new MouseButtonEventHandler(dgr_MouseDoubleClick);
            dataGrid2.Items.Add(dgr);
            int dgr_index = dataGrid2.Items.IndexOf(dgr);

            //Right Click Menu
            ContextMenu m = new System.Windows.Controls.ContextMenu();

            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(del_click2);
            del.Tag = "delete#" + dgr_index;
            m.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(prop_Click);
            prop.Tag = fl.path;
            m.Items.Add(prop);

            //Edit File Name

            VideoFile vfl = new VideoFile();
            vfl.Path = fl.path;
            vfl.Name = fl.Name;
            vfl.Length = fl.len;
            vfl.Ext = fl.path.Substring(fl.path.LastIndexOf('.') + 1);

            fh2.VideoFileList.Add(vfl);
            XMLWriter.WriteFileHistory(fh2, "C:\\PeopleBAWX\\ad_log.xml");

            dgr.ContextMenu = m;
            safl.Add(fl);

        }

        void init_dgr2_search(File_List fl, string dgr_tag)
        {
            if (fl.len.Equals("Automati")) return;

            DataGridRow dgr = new DataGridRow();
            dgr.Tag = dgr_tag;

            dgr.Item = fl;


            //Drag event listeners

            if (File.Exists(fl.path))
            {
                dgr.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.dataGrid1_PreviewMouseLeftButtonDown);
                dgr.PreviewMouseMove += new MouseEventHandler(ddh.dataGrid1_PreviewMouseMove);
            }
            else dgr.ToolTip = "File Does not exist!";

            //Mouse Over Event Listeners
            dgr.MouseEnter += new MouseEventHandler(dgr_MouseEnter);
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave);

            dgr.MouseDoubleClick += new MouseButtonEventHandler(dgr_MouseDoubleClick);
            dataGrid1.Items.Add(dgr);
            int dgr_index = dataGrid1.Items.IndexOf(dgr);

            //Right Click Menu
            ContextMenu m = new System.Windows.Controls.ContextMenu();

            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(del_click);
            del.Tag = "delete#" + dgr_index;
            m.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(prop_Click);
            prop.Tag = fl.path;
            m.Items.Add(prop);

            //Edit File Name

            dgr.ContextMenu = m;
            //safl.Add(fl);

        }

        void del_click2(object sender, RoutedEventArgs e)
        {
            MenuItem del = (MenuItem)sender;
            int idx = int.Parse(del.Tag.ToString().Substring(del.Tag.ToString().LastIndexOf('#') + 1));
            fh2.VideoFileList.RemoveAt(idx);
            XMLWriter.WriteFileHistory(fh2, "C:\\PeopleBAWX\\ad_log.xml");
            dataGrid2.Items.RemoveAt(idx);
            safl.RemoveAt(idx);
        }
        void del_click(object sender, RoutedEventArgs e)
        {
            MenuItem del = (MenuItem)sender;
            int idx = int.Parse(del.Tag.ToString().Substring(del.Tag.ToString().LastIndexOf('#') + 1));
            fh.VideoFileList.RemoveAt(idx);
            XMLWriter.WriteFileHistory(fh, "C:\\PeopleBAWX\\fh_log.xml");
            dataGrid1.Items.RemoveAt(idx);
            svfl.RemoveAt(idx);
        }


        void dgr_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;
            dgr.IsSelected = true;

            File_List a = new File_List();
            a = (File_List)dgr.Item;

            mediaElement1.Source = new Uri(a.path);
            mediaElement1.Play();
        }

        //DataGridRow Mouse Over Events
        DataGridRow dgr = new DataGridRow();
        
        void dgr_MouseEnter(object sender, MouseEventArgs e)
        {
            dgr = (DataGridRow)sender;
            dgr.Background = new SolidColorBrush(Colors.LightGray);

            

            Thickness dg_th = new Thickness();
            dg_th = dataGrid1.Margin;

            int dgr_index = dataGrid1.Items.IndexOf(dgr);
            Point p = e.GetPosition(popup_canvas);
            gPop.Visibility = System.Windows.Visibility.Visible;
            if (p.X >= popup_canvas.ActualWidth - 65)
            {
                gPop.SetValue(LeftProperty, popup_canvas.ActualWidth - 65);
            }
            else
                gPop.SetValue(LeftProperty, p.X + 5);
            gPop.SetValue(TopProperty, p.Y);
            

            gPop.MouseEnter += new MouseEventHandler(popup_MouseEnter);
            gPop.MouseLeave += new MouseEventHandler(popup_MouseLeave);

            File_List temp = new File_List();
            temp = (File_List)dgr.Item;


            Button pl = new Button();
            pl.Content = ">";
            pl.Height = 20;
            pl.Width = 20;
            pl.Click += new RoutedEventHandler(pl_Click);
            // pl.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            pl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            pl.Tag = temp.path;

            Button st = new Button();
            st.Content = "||";
            st.Height = 20;
            st.Width = 20;
            st.Click += new RoutedEventHandler(st_Click);
            st.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //st.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            st.Tag = temp.path;

            Button del = new Button();
            del.Content = "X";
            del.Height = 20;
            del.Width = 20;
            del.Click += new RoutedEventHandler(del_Click);
            del.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //del.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            del.Tag = dgr_index.ToString();

            gPop.Children.Add(pl);
            gPop.Children.Add(st);
            gPop.Children.Add(del);


            //if (main_canvas.Children.IndexOf(popup) == popup_index)
            //{
            //    return;
            //}

            //main_canvas.Children.Add(popup);

            //popup_index = main_canvas.Children.IndexOf(popup);
        }
        void dgr_MouseEnter2(object sender, MouseEventArgs e)
        {
            dgr = (DataGridRow)sender;
            dgr.Background = new SolidColorBrush(Colors.LightGray);


            Thickness dg_th = new Thickness();
            dg_th = dataGrid2.Margin;

            int dgr_index = dataGrid1.Items.IndexOf(dgr);
            Point p = e.GetPosition(popup_canvas);
            if (p.X >= popup_canvas.ActualWidth - 65)
            {
                gPop.SetValue(LeftProperty, popup_canvas.ActualWidth - 65);
            }
            else
                gPop.SetValue(LeftProperty, p.X + 5);
            gPop.SetValue(TopProperty, p.Y);

            gPop.MouseEnter += new MouseEventHandler(popup_MouseEnter);
            gPop.MouseLeave += new MouseEventHandler(popup_MouseLeave);

            File_List temp = new File_List();
            temp = (File_List)dgr.Item;


            Button pl = new Button();
            pl.Content = ">";
            pl.Height = 20;
            pl.Width = 20;
            pl.Click += new RoutedEventHandler(pl_Click);
            // pl.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            pl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            pl.Tag = temp.path;

            Button st = new Button();
            st.Content = "||";
            st.Height = 20;
            st.Width = 20;
            st.Click += new RoutedEventHandler(st_Click);
            st.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //st.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            st.Tag = temp.path;

            Button del = new Button();
            del.Content = "X";
            del.Height = 20;
            del.Width = 20;
            del.Click += new RoutedEventHandler(del_Click2);
            del.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            //del.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            del.Tag = dgr_index.ToString();

            gPop.Children.Add(pl);
            gPop.Children.Add(st);
            gPop.Children.Add(del);


            //if (main_canvas.Children.IndexOf(popup) == popup_index)
            //{
            //    return;
            //}

            //main_canvas.Children.Add(popup);

            //popup_index2 = main_canvas.Children.IndexOf(popup);
        }
        void dgr_MouseLeave(object sender, MouseEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;
            dgr.Background = new SolidColorBrush(Colors.Gray);
            gPop.Visibility = System.Windows.Visibility.Hidden;
        }
        void dgr_MouseLeave2(object sender, MouseEventArgs e)
        {
            DataGridRow dgr = (DataGridRow)sender;
            dgr.Background = new SolidColorBrush(Colors.Gray);
            dgr.MouseLeave -= dgr_MouseLeave2;
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave);
        }
        //POPUP MOUSE OVER EVENTS
        void popup_MouseLeave(object sender, MouseEventArgs e)
        {
            //main_canvas.Children.Remove(popup);
            gPop.Visibility = System.Windows.Visibility.Hidden;
            dgr.MouseLeave += new MouseEventHandler(dgr_MouseLeave2);
        }

        void popup_MouseEnter(object sender, MouseEventArgs e)
        {
            dgr.MouseLeave -= dgr_MouseLeave;
        }

        void st_Click(object sender, RoutedEventArgs e)
        {
            dgr.IsSelected = false;
            mediaElement1.Pause();
        }

        void pl_Click(object sender, RoutedEventArgs e)
        {
            Button pl = (Button)sender;
            mediaElement1.Source = new Uri(pl.Tag.ToString());
            dgr.IsSelected = true;
            mediaElement1.Play();
        }

        void del_Click(object sender, RoutedEventArgs e)
        {
            Button del = (Button)sender;
            int idx = int.Parse(del.Tag.ToString());
            fh.VideoFileList.RemoveAt(idx);
            XMLWriter.WriteFileHistory(fh, "C:\\PeopleBAWX\\fh_log.xml");
            svfl.RemoveAt(idx);
            dataGrid1.Items.RemoveAt(idx);
        }
        void del_Click2(object sender, RoutedEventArgs e)
        {
            Button del = (Button)sender;
            int idx = int.Parse(del.Tag.ToString());
            fh2.VideoFileList.RemoveAt(idx);
            XMLWriter.WriteFileHistory(fh2, "C:\\PeopleBAWX\\ad_log.xml");
            dataGrid2.Items.RemoveAt(idx);
            safl.RemoveAt(idx);
        }


        #endregion

        #region TIMETABLE

        //get dataObject from eventArgs e
        #region DataObject from EventArgs e
        private String[] getData(DragEventArgs e)
        {
            //char[] seperator = new char[] { '#' };
            string[] content = e.Data.GetData("Text").ToString().Split('#');
            return content;
        }
        private void updateData(String[] data, DragEventArgs e)
        {
            MessageBox.Show("did you ever come in?");
            String[] d = data;
            string tag = d[0];
            string path = d[1];
            string time = d[2];
            string newData = tag + "#" + path + "#" + time;
            e.Data.SetData(System.Windows.DataFormats.Text.ToString(), newData);
        }
        private int getLength(String time)
        {
            int hour = int.Parse(time.Substring(0, 2));
            int min = int.Parse(time.Substring(3, 2));
            int sec = int.Parse(time.Substring(6, 2));
            int len = hour * 60 + min;
            int q = len / 30;
            int r = len % 30;
            if (q < 1) return 1;
            else if (r != 0) return q + 1;
            else return q;
        }
        #endregion

        #region TimeTable1 - Day
        private void generate_timetable1() {
            for (int row = 0; row < 48; row++) {
                Grid g = g_init(row);
                table1.Children.Add(g);
            }
        }

        private Grid g_init(int row) {

            Grid g = new Grid();
            g.Tag = " ";
            if (row % 2 == 0)
            {
                g.Background = Brushes.DarkGray;
            }
            else g.Background = Brushes.LightGray;

            ColumnDefinition col0 = new ColumnDefinition();
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            g.ColumnDefinitions.Add(col0);
            g.ColumnDefinitions.Add(col1);
            g.ColumnDefinitions.Add(col2);
            //g.ShowGridLines = true;

            TextBlock name = new TextBlock();
            name.Text = " ";
            name.FontSize = 10;
            name.Foreground = Brushes.White;
            Grid.SetColumn(name, 0);
            g.Children.Add(name);

            TextBlock time = new TextBlock();
            time.Text = " ";
            time.TextAlignment = TextAlignment.Center;
            time.FontSize = 10;
            time.Foreground = Brushes.White;
            Grid.SetColumn(time, 1);
            g.Children.Add(time);

            TextBlock remark = new TextBlock();
            remark.Text = " ";
            remark.FontSize = 10;
            remark.Foreground = Brushes.White;
            Grid.SetColumn(remark, 2);
            g.Children.Add(remark);
            
            Grid.SetRow(g, row);
            Grid.SetColumn(g, 1);
            g.MouseEnter += new MouseEventHandler(g_MouseEnter);
            g.MouseLeave += new MouseEventHandler(g_MouseLeave);
            g.PreviewDragEnter += new DragEventHandler(g_PreviewDragEnter);
            g.PreviewDragLeave += new DragEventHandler(g_PreviewDragLeave);

            return g;
            //table1.Children.Add(g);
        }

        void g_Drop(object sender, DragEventArgs e) {
            
            Grid g = (Grid)sender;
            String[] data = getData(e);
            string tag = data[0];
            string p = data[1];
            string time = data[2];
            //MessageBox.Show(time);
            
            int len = getLength(time);
            int idx = table1.Children.IndexOf(g);

            //delete previous data record
            if (tag.Equals("dgr"))
            {
                //do nothing
            }
            else
            {
                int index = int.Parse(tag.Substring(3));
                int video_idx = index + 48 * previous_day;
                g_cleanPrevious(index, len);
                videoList[video_idx] = null;
            }

            //set new data record
            tag = "hdr" + idx;
            //---------------update header--------------------//
            g.Tag = tag;
            g.Background = brush;
            g.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.g_PreviewMouseLeftButtonDown);
            g.PreviewMouseMove += new MouseEventHandler(ddh.g_PreviewMouseMove);

            //update three columns inside grid
            TextBlock name = (TextBlock)g.Children[0];
            name.Text = MIExtractor.ExtractInfo(p)[0];

            TextBlock duration = (TextBlock)g.Children[1];
            duration.Text = time;

            TextBlock path = (TextBlock)g.Children[2];
            path.Text = data[1];


            //Right Click Menu
            ContextMenu menu = new System.Windows.Controls.ContextMenu();
            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(g_delClick);
            del.Tag = "delete#" + idx;
            menu.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(g_propClick);
            prop.Tag = data[1];
            menu.Items.Add(prop);

            g.ContextMenu = menu;

            //----------------update sub_tb------------------------//
            g_updateSub(g, data);

            //------------------Update Video--------------------/*/
            VideoFile v = new VideoFile();

            v.Path = data[1];
            v.Name = data[1].Substring(data[1].LastIndexOf('\\') + 1);
            v.Length = data[2];
            v.Ext = data[1].Substring(data[1].LastIndexOf('.') + 1);
            v.Col = previous_day;
            v.Row = Grid.GetRow(g);
            v.Index = idx + v.Col*48;

            int hr = v.Row / 2;
            int min;
            if (v.Row % 2 == 0)
            {
                min = 0;
            }
            else min = 30;
            v.Start_Time = hr.ToString() + ":" + min.ToString() + ":00";

            int end_hr, end_min, end_sec;

            end_hr = int.Parse(v.Length.Substring(0, 2)) + hr;
            end_min = int.Parse(v.Length.Substring(3, 2)) + min;
            end_sec = int.Parse(v.Length.Substring(6, 2));

            v.End_Time = end_hr.ToString() + ":" + end_min.ToString() + ":" + end_sec.ToString();

            videoList[v.Index] = v;
        
        }

        void g_delClick(object sender, RoutedEventArgs e) {

            MenuItem del = (MenuItem)sender;
            int idx = int.Parse(del.Tag.ToString().Substring(del.Tag.ToString().LastIndexOf('#') + 1));
            MessageBox.Show("idx = " + idx);
            int video_idx = idx + previous_day * 48;
            Grid g = (Grid)table1.Children[idx];
            TextBlock time = (TextBlock)g.Children[1];
            int len = getLength(time.Text);
            g_cleanPrevious(idx, len);
            videoList[video_idx] = null;

        }
        void g_propClick(object sender, RoutedEventArgs e) {
        
        }

        Boolean g_isAvailable(Grid g, DragEventArgs e)
        {
            
            String[] data = getData(e);
            string tag = data[0];
            string time = data[2];
            int len = getLength(time);
            int idx = table1.Children.IndexOf(g);
            Boolean isAvailable = false;
            
            //check if the following len tb is available for putting the program

            if (len == 1) return true;
            for (int i = 1; i < len; i++)
            {
                if ((idx + i > 71))
                {
                    return false;
                }
                Grid sub_g = (Grid)table1.Children[idx + i];

                if (sub_g.Tag.Equals(" "))
                {
                    //do nothing
                    isAvailable = true;
                }
                else if (tag.Substring(3).Equals(sub_g.Tag.ToString().Substring(3)))
                {

                    //MessageBox.Show("drag up and down");
                    isAvailable = true;
                }
                else if (sub_g.Tag.ToString().Substring(0, 3).Equals("hdr"))
                {
                    //MessageBox.Show("you are reaching another program. there must be something changed");
                    isAvailable = false;
                    break;
                }
                else if (sub_g.Tag.ToString().Substring(0, 3).Equals("sub") && int.Parse(sub_g.Tag.ToString().Substring(3)) == idx)
                {
                    //do nothing
                    isAvailable = true;
                }

                else
                {
                    //MessageBox.Show("oops. I dunno what's happening then");
                    isAvailable = false;
                    break;
                }
            }
            return isAvailable;
        }
        void g_cleanPrevious(int index, int len) {

            Grid hdr = (Grid)table1.Children[index];
            int row = Grid.GetRow(hdr);
            Grid g = g_init(row);
            table1.Children.RemoveAt(index);
            table1.Children.Insert(index, g);

            for (int i = 1; i < len; i++)
            {
                Grid sub = (Grid)table1.Children[index + i];
                sub.Tag = " ";

                if (Grid.GetRow(sub) % 2 == 0)
                    sub.Background = Brushes.DarkGray;
                else 
                    sub.Background = Brushes.LightGray;

            }

        }
        void g_updateSub(Grid g, String[] d) {

            String[] data = d;
            string tag = data[0];
            int idx = table1.Children.IndexOf(g);
            string time = data[2];
            int len = getLength(time);
            if (len > 1)
            {
                for (int i = 1; i < len; i++)
                {
                    Grid sub = (Grid)table1.Children[idx + i];
                    sub.Tag = "sub" + idx;
                    sub.Background = Brushes.Pink;
                }
            }
        }

        void g_PreviewDragLeave(object sender, DragEventArgs e)
        {
            try {

                Grid g = (Grid)sender;
                g.AllowDrop = true;

                if (g.Tag.Equals(" "))
                {
                    int row = Grid.GetRow(g);
                    if (row % 2 == 0)
                    {
                        g.Background = Brushes.DarkGray;
                    }
                    else g.Background = Brushes.LightGray;

                }
                else if (g.Tag.ToString().Substring(0,3).Equals("hdr"))
                {
                    g.Background = brush;
                }
                else if (g.Tag.ToString().Substring(0, 3).Equals("sub"))
                {
                    g.Background = Brushes.Pink;
                }
                else 
                {
                    //do nothing;
                }
                
            }
            catch { throw new NotImplementedException(); }
        }
        void g_PreviewDragEnter(object sender, DragEventArgs e)
        {
            try {

                Grid g = (Grid)sender;
                String[] data = getData(e);
                string tag = data[0];

                if (data.Length <= 1)
                {
                    g.AllowDrop = false;
                    return;
                }

                if (g.Tag.Equals(" ")) 
                {
                    if (g_isAvailable(g, e))
                    {
                        g.Drop +=new DragEventHandler(g_Drop);
                        g.Background = Brushes.Gray;
                    }
                    else 
                    {
                        g.AllowDrop = false;
                    }

                }
                else if (g.Tag.ToString().Substring(0, 3).Equals("hdr")) 
                {
                    g.AllowDrop = false;

                }
                else if (g.Tag.ToString().Substring(0, 3).Equals("sub"))
                {
                    string idx = g.Tag.ToString().Substring(3);
                    if (tag.Substring(3).Equals(idx) && g_isAvailable(g, e))
                    {
                        g.Drop += new DragEventHandler(g_Drop);
                        g.Background = Brushes.Gray;
                    }
                    else
                    {
                        g.AllowDrop = false;
                    }
                }
                else 
                {
                    g.AllowDrop = false;
                }

            }
            catch { throw new NotImplementedException(); }
        }

        void g_MouseLeave(object sender, MouseEventArgs e)
        {
            try {
                Grid g = (Grid)sender;
                int row = Grid.GetRow(g);
                if (g.Background != Brushes.Pink && g.Background != brush)
                {
                    if (row % 2 == 0)
                    {
                        g.Background = Brushes.DarkGray;
                    }
                    else g.Background = Brushes.LightGray;
                }
            }
            catch { throw new NotImplementedException(); }
        }
        void g_MouseEnter(object sender, MouseEventArgs e)
        {
            try {
                Grid g = (Grid)sender;
                if (g.Background != Brushes.Pink && g.Background != brush)
                {
                    g.Background = Brushes.Gray;
                }

                if (g.Background == brush)
                {
                    g.Cursor = Cursors.Hand;
                }
            }
            catch { throw new NotImplementedException(); }
        }

        private int g_getIndex(int idx, int day)
        {

            int g_index = idx - day * 48;
            return g_index;

        }

        #endregion

        #region TimeTable2 - Week
        /*-----------------------------TimeTable2-------------------------------*/
        
        

        private void generate_timetable2()
        {

            for (int i = 1; i < 8; i++)//generate column
            {
                for (int j = 0; j < 48; j++)//generate row
                {
                    TextBlock tb = tb_init(i, j);
                    table2.Children.Add(tb);
                }
            }

            
            dataGrid1.Drop += new DragEventHandler(dataGrid1_Drop);
            dataGrid2.Drop += new DragEventHandler(dataGrid2_Drop);
        }

        private TextBlock tb_init(int col, int row)
        {
            TextBlock tb = new TextBlock();
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            tb.Tag = " ";
            tb.TextWrapping = TextWrapping.Wrap;
            if (row % 2 == 0)
            {

                tb.Background = new SolidColorBrush(Colors.DarkGray);
            }
            else tb.Background = new SolidColorBrush(Colors.LightGray);
            //tb.Drop += new DragEventHandler(tb_Drop);
            tb.MouseEnter += new MouseEventHandler(tb_MouseEnter);
            tb.MouseLeave += new MouseEventHandler(tb_MouseLeave);
            tb.PreviewDragEnter += new DragEventHandler(tb_PreviewDragEnter);
            tb.PreviewDragLeave += new DragEventHandler(tb_PreviewDragLeave);

            return tb;
        }

        void tb_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                TextBlock t = (TextBlock)sender;
                int row = Grid.GetRow(t);
                if (t.Background != brush && t.Background != Brushes.Transparent)
                {
                    if (row % 2 == 0)
                    {

                        t.Background = new SolidColorBrush(Colors.DarkGray);
                    }
                    else t.Background = new SolidColorBrush(Colors.LightGray);
                }
                if (!t.Tag.ToString().Equals(" "))
                {

                    if (t.Tag.ToString().Substring(0, 3).Equals("hdr"))
                    {
                        infoPop.Visibility = System.Windows.Visibility.Hidden;
                        t.MouseMove -= tb_MouseMove;
                    }
                }
            }
            catch { throw new NotImplementedException(); }
        }
        void tb_MouseEnter(object sender, MouseEventArgs e)
        {
          
                TextBlock t = (TextBlock)sender;

                if (t.Background == brush && t.Background != Brushes.Transparent)
                {
                    t.Cursor = Cursors.Hand;
                }

                if (t.Background != brush && t.Background != Brushes.Transparent)
                {

                    t.Background = Brushes.Gray;
                }


                if (!t.Tag.ToString().Equals(" "))
                {


                    if (t.Tag.ToString().Substring(0, 3).Equals("hdr"))
                    {
                        t.MouseMove += new MouseEventHandler(tb_MouseMove);
                    }
                }
           
        }

        void tb_MouseMove(object sender, MouseEventArgs e)
        {
            try 
            {
                TextBlock t = (TextBlock)sender;
                string path = t.Tag.ToString().Split('#')[1];
                
                infoPop.Visibility = System.Windows.Visibility.Visible;
                infoPop.Background = Brushes.SlateGray;
                infoPop.Opacity = 0.9;
                infoPop.MinWidth = 100;
                infoPop.TextWrapping = TextWrapping.Wrap;
                infoPop.Text = popInfo_show(path);
                infoPop.Foreground = Brushes.White;
                infoPop.FontSize = 10;

                Point p = e.GetPosition(popup_canvas);
                infoPop.SetValue(LeftProperty, p.X);
                infoPop.SetValue(TopProperty, p.Y + 20);
            
            }
            catch { throw new NotImplementedException(); }
        }

        
        public void tb_PreviewDragLeave(object sender, DragEventArgs e)
        {
            TextBlock target = (TextBlock)sender;

            // Data information Tag # Path # Time # Row # Col //
            String[] data = getData(e);
            string tag = data[0];
            if (data.Length <= 2)
            {
                target.AllowDrop = false;
                return;
            }
            //string time = data[2];
            //int len = getLength(time);

            int row = Grid.GetRow(target);

            //check condition//

            if (target.Tag.Equals(" "))
            {
                //The target is not occupied
                if (tb_isAvailable(target, e))
                {
                    target.Drop -= tb_Drop;
                    if (target.Background != brush && target.Background != Brushes.Transparent)
                    {
                        if (row % 2 == 0)
                        {

                            target.Background = new SolidColorBrush(Colors.DarkGray);
                        }
                        else target.Background = new SolidColorBrush(Colors.LightGray);
                    }
                }
                else
                {
                    target.AllowDrop = true;
                }

            }
            else if (target.Tag.ToString().Substring(0, 3).Equals("hdr"))
            {
                target.AllowDrop = true;
            }
            else if (target.Tag.ToString().Substring(0, 3).Equals("sub"))
            {

                string idx = target.Tag.ToString().Split('#')[0].Substring(3);
                if (tag.Substring(3).Equals(idx) && tb_isAvailable(target, e))
                {
                    target.Drop -= tb_Drop;
                    target.Background = Brushes.Transparent;
                }
                else
                {
                    target.AllowDrop = true;
                }
            }
            else
            {
                target.AllowDrop = true;
                if (target.Background != brush && target.Background != Brushes.Transparent)
                {
                    if (row % 2 == 0)
                    {

                        target.Background = new SolidColorBrush(Colors.DarkGray);
                    }
                    else
                    {
                        target.Background = new SolidColorBrush(Colors.LightGray);
                    }
                }
            }
        }
        public void tb_PreviewDragEnter(object sender, DragEventArgs e)
        {
            TextBlock target = (TextBlock)sender;
            // Data information Tag # Path # Time # Row # Col //
            String[] data = getData(e);
            string tag = data[0];
            if (data.Length <= 2)
            {
                target.AllowDrop = false;
                return;
            }

            //check condition//

            if (target.Tag.Equals(" "))
            {
                if (tb_isAvailable(target, e))
                {
                    target.Drop += new DragEventHandler(tb_Drop);
                    target.Background = Brushes.Gray;
                }
                else
                {
                    target.AllowDrop = false;
                }

            }
            else if (target.Tag.ToString().Substring(0, 3).Equals("hdr"))
            {
                target.AllowDrop = false;
            }
            else if (target.Tag.ToString().Substring(0, 3).Equals("sub"))
            {
                string idx = target.Tag.ToString().Split('#')[0].Substring(3);
                
                if (tag.Substring(3).Equals(idx) && tb_isAvailable(target, e))
                {
                    target.Drop += new DragEventHandler(tb_Drop);
                    target.Background = Brushes.Gray;
                }
                else
                {
                    target.AllowDrop = false;
                }
            }
            else
            {
                target.AllowDrop = false;
            }
        }

        private string popInfo_show(string path) {
            string text;
            string[] info = MIExtractor.ExtractInfo(path);

            text =  "Title:     " + info[0] + "\n";
            text += "Size:     " + info[1] + "\n";
            text += "Length: " + info[4] + "\n";
            text += "Performer: " + info[3] + "\n";
            text += "Date Created: " + info[2];

            return text;
        
        }

        private void tb_Drop(object sender, DragEventArgs e)
        {

            TextBlock t = (TextBlock)sender;
            
            String[] data = getData(e);
            string tag = data[0];
            string path = data[1];
            string time = data[2];
            int len = getLength(time);
            int idx = table2.Children.IndexOf(t);

            //delete previous data record
            if (tag.Equals("dgr"))
            {
                //do nothing
            }
            else
            {
                int index = int.Parse(tag.Substring(3));
                tb_cleanPrevious(index, len);
                videoList[index] = null;
            }

            infoBug.Text = "indext of t in table2 " + table2.Children.IndexOf(t);
            //set new data record
            tag = "hdr" + idx;

            //updateData(data, e);
            //---------------update header--------------------//
            t.Tag = tag + "#" + data[1] + "#" + data[2];

            

            t.Background = brush;
            t.Foreground = Brushes.White;
            //MessageBox.Show(path);
            t.Text = MIExtractor.ExtractInfo(path)[0];
            t.TextWrapping = TextWrapping.Wrap;
            t.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.header_PreviewMouseLeftButtonDown);
            t.PreviewMouseMove += new MouseEventHandler(ddh.header_PreviewMouseMove);

            //Right Click Menu
            ContextMenu menu = new System.Windows.Controls.ContextMenu();
            //Delete
            MenuItem del = new MenuItem();
            del.Header = "Delete";
            del.Click += new RoutedEventHandler(tb_delClick);
            del.Tag = "delete#" + idx;
            menu.Items.Add(del);

            //Properties
            MenuItem prop = new MenuItem();
            prop.Header = "Properties";
            prop.Click += new RoutedEventHandler(tb_propClick);
            prop.Tag = data[1];
            menu.Items.Add(prop);

            t.ContextMenu = menu;

            //----------------update sub_tb------------------------//
            tb_updateSub(t, data);
            
            
            //------------------Update Video--------------------//
            VideoFile v = new VideoFile();

            v.Path = data[1];
            v.Name = data[1].Substring(data[1].LastIndexOf('\\') + 1);
            v.Length = data[2];
            v.Ext = data[1].Substring(data[1].LastIndexOf('.') + 1);
            v.Col = Grid.GetColumn(t);
            v.Row = Grid.GetRow(t);
            v.Index = idx;
            int hr = v.Row / 2;
            int min;
            if (v.Row % 2 == 0)
            {
                min = 0;
            }
            else min = 30;
            v.Start_Time = hr.ToString() + ":" + min.ToString() + ":00";

            int end_hr, end_min, end_sec;

            end_hr = int.Parse(v.Length.Substring(0, 2)) + hr;
            end_min = int.Parse(v.Length.Substring(3, 2)) + min;
            end_sec = int.Parse(v.Length.Substring(6, 2));

            v.End_Time = end_hr.ToString() + ":" + end_min.ToString() + ":" + end_sec.ToString();

            videoList[idx] = v;
        }

        void tb_delClick(object sender, RoutedEventArgs e)
        {
            MenuItem del = (MenuItem)sender;
            int idx = int.Parse(del.Tag.ToString().Substring(del.Tag.ToString().LastIndexOf('#') + 1));

            TextBlock tb = (TextBlock)table2.Children[idx];
            String[] data = tb.Tag.ToString().Split('#');
            string time = data[2];
            int len = getLength(time);
            tb_cleanPrevious(idx, len);
            videoList[idx] = null;
        }
        void tb_propClick(object sender, RoutedEventArgs e)
        {

        }

        
        private void tb_cleanPrevious(int idx, int len)
        {
            TextBlock hdr = (TextBlock)table2.Children[idx];
            int row = Grid.GetRow(hdr);
            int col = Grid.GetColumn(hdr);
            TextBlock tb = tb_init(col, row);
            table2.Children.RemoveAt(idx);
            table2.Children.Insert(idx, tb);

            for (int i = 1; i < len; i++)
            {
                TextBlock sub = (TextBlock)table2.Children[idx + i];
                sub.Tag = " ";
                if (Grid.GetRow(sub) % 2 == 0) 
                    sub.Background = Brushes.DarkGray;
                else 
                    sub.Background = Brushes.LightGray;
                
            }
        }
        private void tb_updateSub(TextBlock tb, String[] data)
        {
            int idx = table2.Children.IndexOf(tb);
            string time = data[2];
            int len = getLength(time);

            if (len > 1)
            {
                Grid.SetRowSpan(tb, len);

                for (int i = 1; i < len; i++)
                {
                    TextBlock sub = (TextBlock)table2.Children[idx + i];
                    sub.Tag = "sub" + tb.Tag.ToString().Substring(3);
                    sub.Background = Brushes.Transparent;//transparent will block mouse enter/leave event
                }
            }
        }

        //private Boolean tb_isOvernight(int idx, int len) {

        //    Boolean result = false;

        //    for (int i = 1; i < len-1; i++) {

        //        if ((idx + i) / 48 == 0) {
        //            result = true;
        //            break;
        //        }
        //    }
        //    return result;
        
        //}

        private Boolean tb_isAvailable(TextBlock tb, DragEventArgs e)
        {
            TextBlock t = tb;
            String[] data = getData(e);
            string tag = data[0];
            string time = data[2];
            int len = getLength(time);
            int idx = table2.Children.IndexOf(t);
            Boolean isAvailable = false;
            //
            //check if the following len tb is available for putting the program
            if (len == 1) isAvailable = true;

            for (int i = 1; i < len; i++)
            {
                if ((idx + i > 359) || ((idx + i -1 - 23) % 48 == 0)) 
                { 
                    return false; 
                }

                TextBlock sub_tb = (TextBlock)table2.Children[idx + i];
                string sub_tag = sub_tb.Tag.ToString().Split('#')[0];
                
                if (sub_tb.Tag.Equals(" "))
                {
                    //do nothing
                    isAvailable = true;
                }
                else if (tag.Substring(3).Equals(sub_tag.Substring(3)))
                {

                    //MessageBox.Show("drag up and down");
                    isAvailable = true;
                }
                else if (sub_tag.Substring(0, 3).Equals("hdr"))
                {
                    //MessageBox.Show("you are reaching another program. there must be something changed");
                    isAvailable = false;
                    break;
                }
                else if (sub_tag.Substring(0, 3).Equals("sub") && int.Parse(sub_tag.Substring(3)) == idx)
                {
                    //do nothing
                    isAvailable = true;
                }

                else
                {
                    //MessageBox.Show("oops. I dunno what's happening then");
                    isAvailable = false;
                    break;
                }
            }
            return isAvailable;
        }

        #endregion

        #endregion

        #region Calendar_week & Calendar_day

        int _month, _year;
        int previous_day;
        int FIRST_ROW = 24;
        int LAST_ROW = 71;

        void date_init()
        {
            _month = DateTime.Now.Month;
            _year = DateTime.Now.Year;

            schedule_file = get_Monday(DateTime.Today);

        }
        private String[] generate_dates(string d)
        {
            String[] str = new String[7];
            String[] date = d.Split('-');

            int year = int.Parse(date[0]);

            int mon = int.Parse(date[1]);

            int day = int.Parse(date[2]);

            DateTime dt = new DateTime(year, mon, day); 

            for (int i = 0; i < 7; i++)
            {
                str[i] = formatDateYMD(dt.Year, dt.Month, dt.Day);
                dt = dt.AddDays(1);
            }
            return str;
        }

        private string formatDateYMD(int year, int month, int day)
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

        private string formatDateDMY(int day, int month, int year)
        {
            string formattedDate = "";


            if (day.ToString().Length == 1)
                formattedDate += "0" + day.ToString() + "-";
            else
                formattedDate += day.ToString() + "-";
            formattedDate += int_to_month(month) + "-";
            formattedDate += year.ToString();

            return formattedDate;
        }

        private int DayOfWeek_to_int(DateTime d)
        {

            string day = d.DayOfWeek.ToString();

            if (day.Equals("Monday")) return 0;
            else if (day.Equals("Tuesday")) return 1;
            else if (day.Equals("Wednesday")) return 2;
            else if (day.Equals("Thursday")) return 3;
            else if (day.Equals("Friday")) return 4;
            else if (day.Equals("Saturday")) return 5;
            else if (day.Equals("Sunday")) return 6;
            else return 0;
        }
        private string int_to_month(int n)
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
        private int month_to_int(string s)
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
        private string get_Monday(DateTime dt)
        {

            string day = dt.DayOfWeek.ToString();

            TimeSpan ts = new TimeSpan(1, 0, 0, 0);

            if (day.Equals("Tuesday"))
                dt = dt.Subtract(ts);
            if (day.Equals("Wednesday"))
                dt = dt.Subtract(ts).Subtract(ts);
            if (day.Equals("Thursday"))
                dt = dt.Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Friday"))
                dt = dt.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Saturday"))
                dt = dt.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);
            if (day.Equals("Sunday"))
                dt = dt.Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts).Subtract(ts);

            string selected_monday = formatDateYMD(dt.Year, dt.Month, dt.Day);
            return selected_monday;
        }
        private string get_Sunday(DateTime dt)
        {

            string day = dt.DayOfWeek.ToString();

            TimeSpan ts = new TimeSpan(1, 0, 0, 0);

            if (day.Equals("Saturday"))
                dt = dt.Add(ts);
            if (day.Equals("Friday"))
                dt = dt.Add(ts).Add(ts);
            if (day.Equals("Thursday"))
                dt = dt.Add(ts).Add(ts).Add(ts);
            if (day.Equals("Wednesday"))
                dt = dt.Add(ts).Add(ts).Add(ts).Add(ts);
            if (day.Equals("Tuesday"))
                dt = dt.Add(ts).Add(ts).Add(ts).Add(ts).Add(ts);
            if (day.Equals("Monday"))
                dt = dt.Add(ts).Add(ts).Add(ts).Add(ts).Add(ts).Add(ts);

            return formatDateYMD(dt.Year, dt.Month, dt.Day);

        }

        private DateTime create_Date(string s)
        {

            int year = int.Parse(s.Split('-')[0]);
            int month = int.Parse(s.Split('-')[1]);
            int day = int.Parse(s.Split('-')[2]);

            DateTime dt = new DateTime(year, month, day);

            return dt;
        }


        private void calendar_week_generate()
        {
            calendar_week.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar_week.FirstDayOfWeek = DayOfWeek.Monday;
            calendar_week_select_week(calendar_week.DisplayDate);
            calendar_week.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_week_SelectedDatesChanged);
        }
        private void calendar_day_generate()
        {
            calendar_day.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar_day.FirstDayOfWeek = DayOfWeek.Monday;
            calendar_day.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_day_SelectedDatesChanged);
        }


        private void calendar_week_cleanTable()
        {

            foreach (VideoFile v in videoList)
            {
                if (v != null)
                {
                    int idx = v.Index;
                    int len = getLength(v.Length);
                    tb_cleanPrevious(idx, len);
                    //videoList[idx] = null;
                }
            }
        }
        private void calendar_week_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            calendar_week.SelectedDatesChanged -= calendar_week_SelectedDatesChanged;

            DateTime dt = calendar_week.SelectedDate.Value;
            s_save();

            calendar_week_cleanTable();

            //directing to new week file, schedule_file is updated inside the function Orz.
            calendar_week_select_week(dt);
            s_init();
            dailyList = get_DailyScheduleList("C:\\PeopleBAWX\\" + schedule_file + ".xml");
            s_loadWeek(dailyList);

            calendar_week.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_week_SelectedDatesChanged);
        }

        private void calendar_day_cleanTable()
        {

            int day = previous_day;
            foreach (VideoFile v in videoList)
            {
                if ((v != null) && (g_getIndex(v.Index, day) <= LAST_ROW) && (g_getIndex(v.Index, day) >= FIRST_ROW))
                {

                    int g_index = g_getIndex(v.Index, day);
                    int len = getLength(v.Length);
                    g_cleanPrevious(g_index, len);
                }
            }
        }
        private void calendar_day_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

            //save current week file
            s_save();
            //clean previous day record
            calendar_day_cleanTable();


            DateTime date = calendar_day.SelectedDate.Value;
            calendar_day_display(date);
            schedule_file = get_Monday(date);
            
            //directing to new week file
            s_init();
            dailyList = get_DailyScheduleList("C:\\PeopleBAWX\\" + schedule_file + ".xml");
            s_loadDay(dailyList, calendar_day.SelectedDate.Value);
            previous_day = DayOfWeek_to_int(date);

        }

        private void calendar_week_select_week(DateTime dt)
        {
            DateTime sdt = new DateTime();
            
            sdt = dt;
            int subtract = 0;
            int add = 0;
            if (sdt.DayOfWeek.ToString().Equals("Monday"))
            {
                subtract = 0;
            }
            if (sdt.DayOfWeek.ToString().Equals("Tuesday"))
            {
                subtract = 1;
            }
            if (sdt.DayOfWeek.ToString().Equals("Wednesday"))
            {
                subtract = 2;
            }
            if (sdt.DayOfWeek.ToString().Equals("Thursday"))
            {
                subtract = 3;
            }
            if (sdt.DayOfWeek.ToString().Equals("Friday"))
            {
                subtract = 4;
            }
            if (sdt.DayOfWeek.ToString().Equals("Saturday"))
            {
                subtract = 5;
            }
            if (sdt.DayOfWeek.ToString().Equals("Sunday"))
            {
                subtract = 6;
            }
            add = 6 - subtract;
            TimeSpan ts = new TimeSpan(1, 0, 0, 0, 0);

            for (int i = 0; i < add; i++)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek.ToString().Equals("Monday"))
                {
                    schedule_file = formatDateYMD(dt.Year, dt.Month, dt.Day);
                }
                calendar_week.SelectedDates.Add(dt);
            }
            dt = sdt;
            for (int j = 0; j < subtract; j++)
            {
                dt = dt.Subtract(ts);
                if (dt.DayOfWeek.ToString().Equals("Monday"))
                {
                    schedule_file = formatDateYMD(dt.Year, dt.Month, dt.Day);
                }
                calendar_week.SelectedDates.Add(dt);
            }

            calendar_week_display(dt);
        }

        private void calendar_week_display(DateTime dt) {
            string[] mon;
            string[] sun;
            mon = get_Monday(dt).Split('-');
            sun = get_Sunday(dt).Split('-');
            if (mon[1] != sun[1])
            {
                week_disp.Text = mon[2] + int_to_month(int.Parse(mon[1])) + '-' + sun[2] + int_to_month(int.Parse(sun[1]));
            }
            else
            {
                week_disp.Text = mon[2] + '-' + sun[2] + ' ' + int_to_month(int.Parse(mon[1]));
            }
        }
        private void calendar_day_display(DateTime dt)
        {
            week_disp.Text = int_to_month(dt.Month) + ' ' + dt.Day + ',' + dt.Year;
        }

        private void calendar_week_button(object sender, MouseButtonEventArgs e)
        {
            if (currentTab.Equals("day"))
            {
                if (calendar_day.Visibility.Equals(System.Windows.Visibility.Hidden))
                    calendar_day.Visibility = System.Windows.Visibility.Visible;
                else calendar_day.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (currentTab.Equals("week"))
            {
                if (calendar_week.Visibility.Equals(System.Windows.Visibility.Hidden))
                    calendar_week.Visibility = System.Windows.Visibility.Visible;
                else calendar_week.Visibility = System.Windows.Visibility.Hidden;
            }
            else { }

        }


        #endregion

        #region Loading Schedule
        Schedule sc = new Schedule();
        string schedule_file;
        DailyVideoFiles[] dailyList = new DailyVideoFiles[7];
        VideoFile[] videoList;

        private void s_init()
        {
            videoList = new VideoFile[table2.Children.Count];

            for (int i = 0; i < 7; i++)
            {
                dailyList[i] = new DailyVideoFiles();
            }
        }
        private DailyVideoFiles[] get_DailyScheduleList(string f)
        {
            Schedule s = new Schedule();
            s = XMLReader.ReadSchedule(f);
            List<DailyVideoFiles> list = s.DailyVideoFilesList;

            list.CopyTo(dailyList, 0);
            return dailyList;
        }

        private void s_loadDay(DailyVideoFiles[] list, DateTime d)
        {

            int day = DayOfWeek_to_int(d);

            foreach (DailyVideoFiles dailyFile in list)
            {
                foreach (VideoFile v in dailyFile.VideoFileList)
                {
                    videoList[v.Index] = v;

                    int g_index = g_getIndex(v.Index, day);

                    if ((FIRST_ROW <= g_index) && (g_index <= LAST_ROW))
                    {

                        Grid g = (Grid)table1.Children[g_index];
                        //---------------update header--------------------//
                        g.Tag = "hdr" + g_index;
                        g.Background = brush;
                        g.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.g_PreviewMouseLeftButtonDown);
                        g.PreviewMouseMove += new MouseEventHandler(ddh.g_PreviewMouseMove);

                        //update three columns inside grid
                        TextBlock name = (TextBlock)g.Children[0];
                        TextBlock duration = (TextBlock)g.Children[1];
                        TextBlock path = (TextBlock)g.Children[2];
                        name.Text = name.Text = MIExtractor.ExtractInfo(v.Path)[0];
                        duration.Text = v.Length;
                        path.Text = v.Path;


                        //Right Click Menu
                        ContextMenu menu = new System.Windows.Controls.ContextMenu();
                        //Delete
                        MenuItem del = new MenuItem();
                        del.Header = "Delete";
                        del.Click += new RoutedEventHandler(g_delClick);
                        del.Tag = "delete#" + g_index;
                        menu.Items.Add(del);

                        //Properties
                        MenuItem prop = new MenuItem();
                        prop.Header = "Properties";
                        prop.Click += new RoutedEventHandler(g_propClick);
                        prop.Tag = v.Path;
                        menu.Items.Add(prop);

                        g.ContextMenu = menu;

                        //----------------update sub_tb------------------------//
                        String[] data = new String[3];
                        data[0] = g.Tag.ToString();
                        data[1] = v.Path;
                        data[2] = v.Length;
                        g_updateSub(g, data);
                    }
                }
            }
        }
        private void s_loadWeek(DailyVideoFiles[] list)
        {


            foreach (DailyVideoFiles dailyFile in list)
            {
                foreach (VideoFile v in dailyFile.VideoFileList)
                {
                    int index = v.Index;
                    videoList[index] = v;

                    TextBlock t = (TextBlock)table2.Children[index];
                    t.Tag = "hdr" + index + "#" + v.Path + "#" + v.Length;


                    t.Background = brush;
                    t.Foreground = Brushes.White;
                    t.Text = MIExtractor.ExtractInfo(v.Path)[0];
                    t.TextWrapping = TextWrapping.Wrap;
                    t.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ddh.header_PreviewMouseLeftButtonDown);
                    t.PreviewMouseMove += new MouseEventHandler(ddh.header_PreviewMouseMove);

                    //Right Click Menu
                    ContextMenu menu = new System.Windows.Controls.ContextMenu();
                    //Delete
                    MenuItem del = new MenuItem();
                    del.Header = "Delete";
                    del.Click += new RoutedEventHandler(tb_delClick);
                    del.Tag = "delete#" + index;
                    menu.Items.Add(del);

                    //Properties
                    MenuItem prop = new MenuItem();
                    prop.Header = "Properties";
                    prop.Click += new RoutedEventHandler(tb_propClick);
                    prop.Tag = v.Path;
                    menu.Items.Add(prop);

                    t.ContextMenu = menu;

                    //----------------update sub_tb------------------------//
                    String[] data = new String[3];
                    data[0] = t.Tag.ToString();
                    data[1] = v.Path;
                    data[2] = v.Length;
                    tb_updateSub(t, data);

                }
            }
        }
        private void s_save()
        {
            //loadig the week schedule copy to the videoList and dailyList
            sc.DailyVideoFilesList = new List<DailyVideoFiles>();
            String[] dates = new String[7];
            dates = generate_dates(schedule_file);

            for (int i = 0; i < 7; i++)
            {
                dailyList[i] = new DailyVideoFiles();
            }

            foreach (VideoFile video in videoList)
            {
                if (video != null)
                {
                    int day = video.Col - 1;
                    dailyList[day].Date = dates[day];
                    dailyList[day].VideoFileList.Add(video);
                }
            }
            for (int i = 0; i < 7; i++)
            {
                sc.DailyVideoFilesList.Add(dailyList[i]);
            }
            //MessageBox.Show("this is schedule_file " + schedule_file);
            string file = "C:\\PeopleBAWX\\" + schedule_file + ".xml";
            XMLWriter.WriteSchedule(sc, file);

        }

        private void s_save_export()
        {
            //loadig the week schedule copy to the videoList and dailyList
            sc.DailyVideoFilesList = new List<DailyVideoFiles>();
            String[] dates = new String[7];
            dates = generate_dates(schedule_file);

            for (int i = 0; i < 7; i++)
            {
                dailyList[i] = new DailyVideoFiles();
            }

            VideoFile v = new VideoFile();
            foreach (VideoFile video in videoList)
            {
                if (video != null)
                {
                    v.Path = video.Path.Substring(video.Path.LastIndexOf("\\") + 1);
                    v.Name = video.Name;
                    v.Ext = video.Ext;
                    v.Length = video.Length;
                    v.Start_Time = video.Start_Time;
                    v.End_Time = video.End_Time;
                    v.Col = video.Col;
                    v.Row = video.Row;
                    v.Index = video.Index;

                    int day = video.Col - 1;
                    dailyList[day].Date = dates[day];
                    dailyList[day].VideoFileList.Add(v);
                }
            }
            for (int i = 0; i < 7; i++)
            {
                sc.DailyVideoFilesList.Add(dailyList[i]);
            }
            //MessageBox.Show("this is schedule_file " + schedule_file);
            string file = "C:\\PeopleBAWX\\" + schedule_file + "_s.xml";
            XMLWriter.WriteSchedule(sc, file);
        }


        #endregion

        #region EventHandler

        #region DataStructure Handler Functions

        Uri get_img_src(string path)
        {
            Uri src;
            if (!File.Exists(path))
            {
                src = new Uri(System.Environment.CurrentDirectory.ToString() + "..\\Assets\\fail.jpg");
            }
            else src = new Uri(System.Environment.CurrentDirectory.ToString() + "..\\Assets\\wmv.png");
            return src;
        }

        void read_filehistory()
        {
            FileHistory fileHistory = new FileHistory();

            fileHistory = XMLReader.ReadFileHistory("C:\\PeopleBAWX\\fh_log.xml");
            VideoFile[] vf = new VideoFile[1024];

            fileHistory.VideoFileList.CopyTo(vf);

            File_List vfl = new File_List();

            for (int i = 0; vf[i] != null; i++)
            {
                Uri src = get_img_src(vf[i].Path);
                
                //MessageBox.Show(src.ToString());
                

                vfl = (new File_List()
                {
                    Name = vf[i].Name,
                    path = vf[i].Path,
                    len = vf[i].Length,
                    img = new BitmapImage(src),
                });

                string dgr_tag = vf[i].Path + "#" + vf[i].Length;

                init_dgr(vfl, dgr_tag);
            }
        }
        void read_adhistory()
        {
            FileHistory fileHistory = new FileHistory();

            fileHistory = XMLReader.ReadFileHistory("C:\\PeopleBAWX\\ad_log.xml");
            VideoFile[] vf = new VideoFile[1024];

            fileHistory.VideoFileList.CopyTo(vf);

            File_List vfl = new File_List();

            for (int i = 0; vf[i] != null; i++)
            {

                Uri src = get_img_src(vf[i].Path);

                //mediaElement1.Source = new Uri(vf[i].Path.ToString());

                vfl = (new File_List()
                {
                    Name = vf[i].Name,
                    path = vf[i].Path,
                    len = vf[i].Length,
                    img = new BitmapImage(src),
                });

                string dgr_tag = vf[i].Path + "#" + vf[i].Length;

                init_dgr2(vfl, dgr_tag);
            }
        }

        #endregion

        #region Button Click


        void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement1.Volume = slider1.Value * 0.1;
        }

        private String[] export_WeekSchedule()
        {

            String[] list = new String[videoList.Count()];

            foreach (VideoFile video in videoList)
            {
                if (video != null)
                {
                    int day = video.Col - 1;
                    //MessageBox.Show(day.ToString());
                    list[day] = video.Path;
                }
            }
            return list;
        }

        //tab
        string currentTab = "week";

        private void day_GotFocus(object sender, RoutedEventArgs e)
        {
            if (currentTab.Equals("week"))
            {
                
                //clean up the table
                calendar_week_cleanTable();
            }
            s_save();
            calendar_week.Visibility = System.Windows.Visibility.Hidden;
            //calendar_day.Visibility = System.Windows.Visibility.Visible;
            calendar_day.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_day_SelectedDatesChanged);
            currentTab = "day";

            //selected_monday = current week Monday
            schedule_file = get_Monday(DateTime.Today);
            calendar_day_display(DateTime.Today);

            //loading TODAY's day_schedule
            s_init();
            dailyList = get_DailyScheduleList("C:\\PeopleBAWX\\" + schedule_file + ".xml");
            s_loadDay(dailyList, DateTime.Today);

            //set today as previous_day for next selected date lol
            previous_day = DayOfWeek_to_int(DateTime.Today);

        }
        private void day_LostFocus(object sender, RoutedEventArgs e)
        {

            //save current schedule_file with changes in currentDay
            s_save();

            //hide calendar_day
            calendar_day.Visibility = System.Windows.Visibility.Hidden;

        }

        private void week_GotFocus(object sender, RoutedEventArgs e)
        {
            if (currentTab.Equals("day"))
            {
                
                //clean up the table
                calendar_day_cleanTable();
            }
            s_save();
            //calendar_week.Visibility = System.Windows.Visibility.Visible;
            calendar_day.Visibility = System.Windows.Visibility.Hidden;
            currentTab = "week";
            //schedule_file = selected_monday;
            s_init();
            dailyList = get_DailyScheduleList("C:\\PeopleBAWX\\" + schedule_file + ".xml");
            s_loadWeek(dailyList);
            //normal save


            DateTime dt = create_Date(schedule_file);
            
            calendar_week_display(dt);
        }

        


        private void week_LostFocus(object sender, RoutedEventArgs e)
        {
            //save current schedul_file
            s_save();

            //hide calendar_week
            calendar_week.Visibility = System.Windows.Visibility.Hidden;
        }

        private void month_GotFocus(object sender, RoutedEventArgs e)
        {
            currentTab = "month";
            //no save
            //calendar click_right click_left will have event_handler
        }

        //tool bar
        private void save_Click(object sender, RoutedEventArgs e)
        {
            s_save();

        }

        private void print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dvd_Click(object sender, RoutedEventArgs e)
        {
            s_save();
            BurnDVD.remove_all();
            String[] paths = export_WeekSchedule();

            s_save_export();
            BurnDVD.setVolume_label(schedule_file);
            BurnDVD.file_add("C:\\PeopleBAWX\\" + schedule_file + "_s.xml");

            
            for (int i = 0; i < paths.Count(); i++)
            {

                if (paths[i] == null) continue;

                //MessageBox.Show(paths[i]);
                
                BurnDVD.file_add(paths[i]);
            }
            BurnDVD.burn();
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {

        }

        //media player
        private void play_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Play();
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Pause();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
        }

        private void volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement1.Volume = slider1.Value * 0.1;
        }

        //calendar

        //file history list
        private void ad_Click(object sender, RoutedEventArgs e)
        {
            dataGrid2.Visibility = System.Windows.Visibility.Visible;
            dataGrid1.Visibility = System.Windows.Visibility.Hidden;
           // category.Visibility = System.Windows.Visibility.Visible;
        }

        private void show_Click(object sender, RoutedEventArgs e)
        {
            dataGrid2.Visibility = System.Windows.Visibility.Hidden;
            dataGrid1.Visibility = System.Windows.Visibility.Visible;
            //category.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        #region Search
        void search_box_KeyUp(object sender, KeyEventArgs e)
        {
            search_Click(sender, e);
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            string stxt = search_box.Text;
            stxt = stxt.ToLower();

            close_search_Click(sender, e);

            if (dataGrid2.Visibility == System.Windows.Visibility.Visible)
            {
                for (int j = 0; j < dataGrid2.Items.Count; )
                {
                    DataGridRow dg = (DataGridRow)dataGrid2.Items.GetItemAt(j);
                    File_List vfl = (File_List)dg.Item;

                    if (!vfl.Name.ToLower().Contains(stxt))
                    {
                        dataGrid2.Items.RemoveAt(j);
                    }
                    else j++;
                }
                
            }

            else if (dataGrid2.Visibility == System.Windows.Visibility.Hidden)
            {
                for (int j = 0; j < dataGrid1.Items.Count; )
                {
                    DataGridRow dg = (DataGridRow)dataGrid1.Items.GetItemAt(j);
                    File_List vfl = (File_List)dg.Item;

                    if (!vfl.Name.ToLower().Contains(stxt))
                    {
                        dataGrid1.Items.RemoveAt(j);
                    }
                    else j++;
                }
            }
        }

        private void close_search_Click(object sender, RoutedEventArgs e)
        {

            if (dataGrid2.Visibility == System.Windows.Visibility.Visible)
            {

                for (int j = 0; j < dataGrid2.Items.Count; )
                {
                    dataGrid2.Items.RemoveAt(0);
                }

                // MessageBox.Show(svfl.Count.ToString());
                for (int i = 0; i < safl.Count; i++)
                {
                    File_List vfl = safl.ElementAt(i);
                    //DataGridRow dg = new DataGridRow();
                    //dg.Item = vfl;
                    //MessageBox.Show(vfl.Name);
                    string tag = vfl.path + "#" + vfl.len;
                    init_dgr2_search(vfl, tag);
                }
            }

            else if (dataGrid2.Visibility == System.Windows.Visibility.Hidden)
            {

                for (int j = 0; j < dataGrid1.Items.Count; )
                {
                    dataGrid1.Items.RemoveAt(0);
                }

                // MessageBox.Show(svfl.Count.ToString());
                for (int i = 0; i < svfl.Count; i++)
                {
                    File_List vfl = svfl.ElementAt(i);

                    string tag = vfl.path + "#" + vfl.len;
                    init_dgr_search(vfl, tag);
                }
            }
        }
        #endregion

        
        
        #endregion


    }
}


public class week_view
{
    public string mo { get; set; }
    public string tu { get; set; }
    public string we { get; set; }
    public string th { get; set; }
    public string fr { get; set; }
    public string sa { get; set; }
    public string su { get; set; }
    public string month { get; set; }
}

public class File_List
{
    public string Name { get; set; }
    public string path { get; set; }
    public string len { get; set; }
    public BitmapImage img { get; set; }
    public int idx { get; set; }

}