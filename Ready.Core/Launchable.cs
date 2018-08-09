using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;

namespace Ready.Core
{
    public class Launchable : INotifyPropertyChanged
    {
        public Launchable(string executable, string arguments, string displayName = null, int delay = 0)
        {
            Executable = executable;
            Arguments = arguments;
            DisplayName = displayName ?? executable;
            Delay = Math.Max(0, delay);

            Process = new Process();
            Process.StartInfo = new ProcessStartInfo(Executable, Arguments) { WindowStyle = ProcessWindowStyle.Hidden };

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
            Task.Factory.StartNew(() =>
            {
                Process.Exited += Process_Exited;
               Process.Start();
               SetStatus(Status.Launching);
            }).ContinueWith(t =>
            {
               Thread.Sleep(Delay * 1000);
               SetStatus(Status.Available);
            });
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Proc exited.")
;       }

        internal void SetStatus(Status status)
        {
            if (this.status == status)
                return;

            this.status = status;
            Notify("Status");
        }
        private Status status;

        public Status Status { get { return status; } }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

