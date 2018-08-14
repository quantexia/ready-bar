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
            lmvm = new LaunchableMonitorViewModel(AppController.Monitor);
            this.DataContext = lmvm;
        }

    }
}
