using Ready.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ready.UI.Controls
{
    public class LauncherViewModel
    {
        public LauncherViewModel()
        {
            SetCommands();
        }

        private void SetCommands()
        {
            InstanceUp = new RelayCommand<Launchable>(l => l.Reveal(), l => l.Status == Status.Available);
        }

        public ObservableCollection<Launchable> Targets { get; set;}

        public ICommand InstanceUp { get; private set; }
    }
}
