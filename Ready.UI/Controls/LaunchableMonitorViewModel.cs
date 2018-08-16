using Ready.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ready.UI.Controls
{
    public class LaunchableMonitorViewModel : INotifyPropertyChanged
    {
        private LaunchableMonitor lmon;
        private CancellationToken ct;
        
        public LaunchableMonitorViewModel(LaunchableMonitor lmon)
        {
            this.lmon = lmon;
            LaunchCommand = new RelayCommand(o => lmon.NextAvailable.Reveal(), o => !shutdown && lmon.HasAvailable);
            CloseCommand = new RelayCommand(o => OnClose(), o => true);

            ct = new CancellationToken();
            PeriodicTaskFactory.Start(() => Check(), 1000 * 1, cancelToken: ct);
        }

        public Launchable Launchable { get { return lmon.Target; } }

        private void Check()
        {
            if (DisplayName != lmon.Target.DisplayName) { DisplayName = lmon.Target.DisplayName; Notify("DisplayName"); }
            if (HasAvailable != lmon.HasAvailable) { HasAvailable = lmon.HasAvailable; Notify("HasAvailable"); }
            if (CountAvailable != lmon.CountAvailable) { CountAvailable= lmon.CountAvailable; Notify("CountAvailable"); }
            if (NextPid != lmon.NextPid) { NextPid = lmon.NextPid; Notify("NextPid"); }
        }

        public string DisplayName { get; set; }
        public bool HasAvailable { get; set; }
        public int CountAvailable { get; set; }
        public int NextPid { get; set; }

        public ImageSource Image { get { return lmon.Image; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LaunchCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }


        private bool shutdown;
        private void OnClose()
        {
            shutdown = true;
            lmon.Dispose();
        }
    }
}
