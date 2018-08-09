using Ready.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ready.UI.Controls
{
    public class LauncherViewModel
    {
        public LauncherViewModel()
        { }

        public ObservableCollection<Launchable> Targets { get; set;}
    }
}
