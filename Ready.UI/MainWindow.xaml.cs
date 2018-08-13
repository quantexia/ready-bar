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

#if !DEBUG
            this.spButtons.Visibility = Visibility.Collapsed;
#endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeVM();
        }

        private void InitializeVM()
        {
            lmon = new LaunchableMonitor();
            Launchable lb = new Launchable(
                Configuration.GetValue("Executable"),
                Configuration.GetValue("Arguments")
                );
            lmon.Provision(lb, Configuration.GetValue<int>("Provision"));

            lmvm = new LaunchableMonitorViewModel(lmon);
            this.DataContext = lmvm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = ((Button)sender).Content.ToString();
            

            if (content == "Add")
            {
                if (lmon.ProvisionLevel > 20)
                    return;

                lmon.SetProvisionLevel(lmon.ProvisionLevel + 1);
            }
            if (content == "Remove")
            {
                if (lmon.ProvisionLevel == 0)
                    return;

                lmon.SetProvisionLevel(lmon.ProvisionLevel - 1);
            }

            if (content == "Ex")
            {
                string summary =string.Join("\n", lmon.Launchables.Select(l => string.Format("{0} {1}", l.Process.Id, l.Status)));
                MessageBox.Show(summary);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lmon.Dispose();
        }
    }
}
