using Ready.Core;
using Ready.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ready.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = ((Button)sender).Content.ToString();
            LauncherViewModel lvm = (LauncherViewModel)this.lsv.DataContext;

            if (content == "Init")
            {
                lvm = new LauncherViewModel();
                lvm.Targets = new List<Launchable>
                {
                    new Launchable() { Executable = "notepad.exe", Arguments = "", Delay = 5}
                };
            }
            if (content = "Go")
            {
                Launchable lcb = lvm.Targets.First();
                lcb.Launch();
            }
        }
    }
}
