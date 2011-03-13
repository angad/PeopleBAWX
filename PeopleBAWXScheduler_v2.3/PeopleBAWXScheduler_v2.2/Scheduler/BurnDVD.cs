using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BurnMedia;

namespace Scheduler
{
    class BurnDVD
    {
        static BurnMediaForm bmf = new BurnMediaForm();

        public static void burn()
        {
            bmf.ShowDialog();
        }

        public static void file_add(string file)
        {
            bmf.add_files(file);
        }

        public static void remove_all()
        {
            bmf.remove_all();
        }

        public static void setVolume_label(string schedule)
        {
            bmf.setVolume_label(schedule);
        }

        public static void copy_files(string source, string dest)
        {
            bmf.copy_files(source, dest);
        }
    }
}
