using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ready.Core
{
    public class LaunchableMonitor
    {
        private ObservableCollection<Launchable> coll;

        public LaunchableMonitor()
        {
            coll = new ObservableCollection<Launchable>();
        }

        public void Provision(Launchable lb, int quantity)
        {
            var pr = new List<Launchable> { lb, lb.Clone(), lb.Clone() };
            pr.ForEach(l => l.HasExited += OnLaunchableExit );

        }

        public ObservableCollection<Launchable> Launchables { get { return coll; } }

        private void OnLaunchableExit(object sender, EventArgs e)
        {
            Launchable src = (Launchable)sender;
            Launchable next = src.Clone();
            coll.Add(next);
        }
    }
}
