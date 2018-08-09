using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Ready.Core
{
    public class Launchable
    {
        public Launchable()
        {
            SetStatus(Status.None);
        }
        public string Executable { get; private set; }
        public string Arguments { get; private set; }
        public int Delay { get; private set; }
        public string DisplayName { get; private set; }
        public Image Icon { get; private set; }
        public Process Process { get; private set; }
        public void Launch()
        {
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(Executable, Arguments) { WindowStyle = ProcessWindowStyle.Hidden };

            Process = proc;

            Task.Factory.StartNew(() =>
            {
               proc.Start();
               SetStatus(Status.Launching);
            }).ContinueWith(t =>
            {
               Thread.Sleep(Delay * 1000);
               SetStatus(Status.Available);
            });
        }
        internal void SetStatus(Status status)
        {
            this.status = status;
        }
        private Status status;
        public Status Status { get { return status; } }
    }
}

