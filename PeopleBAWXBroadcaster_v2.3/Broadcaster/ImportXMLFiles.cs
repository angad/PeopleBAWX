using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Broadcaster
{
    public partial class Import : Form
    {
        public Import()
        {
            InitializeComponent();
        }

        private List<string> f_list = new List<string>();
        string dest_path = "C:\\PeopleBAWX\\";
        private void source_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowDialog(this);
            source_path.Text = fb.SelectedPath;
            int flag=0;
            foreach(string s in Directory.GetFiles(fb.SelectedPath))
            {
                string ext = s.Substring(s.LastIndexOf(".") + 1);
                if (ext == "xml")
                {
                    flag = 1;
                    f_list.Add(s);
                }

                if (ext == "wmv" || ext == "avi" || ext == "jpg" || ext == "mpg")
                {
                    f_list.Add(s);
                }
            }

            if(flag == 0)
            {
                MessageBox.Show("No XML Schedule found in the Given Source!");
                source_path.Text = "";
            }
        }

        public void copy_files(string source, string dest)
        {

            string des_name = Path.GetFileName(source);
            string des_path = Path.Combine(dest, des_name);
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            File.Copy(source, des_path, true);
        }

        private void imp_Click(object sender, EventArgs e)
        {

            string dest = dest_path;
            file_Copy.Maximum = 200;
            file_Copy.Minimum = 0;
            int items = f_list.Count;

            file_Copy.Step = 200 / items;

            foreach (string i in f_list)
            {
                copy_files(i, dest);
                file_Copy.Value += file_Copy.Step;
            }

            MessageBox.Show("Files Imported Successfully to " + dest_path);
            file_Copy.Value = 0;
        }
    }
}
