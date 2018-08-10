using Ready.Core;
using Ready.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private LaunchableMonitorViewModel lmvm;
        private LaunchableMonitor lmon;

        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += (x, y) => this.HideMinimizeAndMaximizeButtons();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = ((Button)sender).Content.ToString();
            
            if (content == "Init")
            {
                lmon = new LaunchableMonitor();
                //Launchable lb =new Launchable("notepad.exe", "", "Notepad")
                Launchable lb = new Launchable("Excel.exe", "", "Excel");
                lmon.Provision(lb, 3);

                lmvm = new LaunchableMonitorViewModel(lmon);
                this.vu.DataContext = lmvm;
            }

            if (content == "Add")
            {
                lmon.SetProvisionLevel(lmon.ProvisionLevel + 1);
            }
            if (content == "Remove")
            {
                lmon.SetProvisionLevel(lmon.ProvisionLevel - 1);
            }

            if (content == "Ex")
            {
                
            }
        }
    }
}
