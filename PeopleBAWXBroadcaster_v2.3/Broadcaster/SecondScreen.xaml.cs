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

namespace Broadcaster
{
    /// <summary>
    /// Interaction logic for SecondScreen.xaml
    /// </summary>
    public partial class SecondScreen : Window
    {
        public SecondScreen()
        {
            InitializeComponent();
        }

        public void SetVideoSource(string uri)
        {
            mediaElement1.Source = new Uri(uri);
            mediaElement1.Volume = 0.0;
        }

    }
}
