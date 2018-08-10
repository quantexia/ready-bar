using Ready.Core;
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

namespace Ready.UI.Controls
{
    /// <summary>
    /// Interaction logic for LauncherStackView.xaml
    /// </summary>
    public partial class LauncherStackView : UserControl
    {
        public LauncherStackView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Launchable src = (Launchable)((ListViewItem)sender).DataContext;
            LauncherViewModel lvm = (LauncherViewModel)this.DataContext;
            if (lvm.InstanceUp.CanExecute(src))
                lvm.InstanceUp.Execute(src);
        }
    }
}
