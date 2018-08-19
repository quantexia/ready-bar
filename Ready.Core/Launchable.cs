using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using Serilog;

namespace Ready.Core
{
    public class Launchable : INotifyPropertyChanged, IDisposable
    {
        private ILogger log = Log.Logger.ForContext<Launchable>();

        public Launchable(string executable, string arguments, string displayName = null, int delay = 0, int separation = 0)
        {
            log.Debug("c'tor");

            FileInfo fiExec = new FileInfo(PathHelper.FindExePath(executable));

            log.Debug("Launchable created for {0}", fiExec.FullName);

            FullPath = fiExec.FullName;
            Executable = fiExec.Name;
            Arguments = arguments;
            DisplayName = string.IsNullOrEmpty(displayName) ? Executable : displayName;
            Delay = Math.Max(0, delay);
            Separation = Math.Max(0, separation);

            Process = new Process();
            Process.StartInfo = new ProcessStartInfo(FullPath, Arguments) {
                                            WindowStyle = ProcessWindowStyle.Minimized};

            SetStatus(Status.None);
        }

        public string Executable { get; private set; }
        public string FullPath { get; private set; }
        public string Arguments { get; private set; }
        public int Delay { get; private set; }
        public int Separation { get; private set; }
        public string DisplayName { get; private set; }
        public Process Process { get; private set; }
        public int Pid { get; private set; }

        public void Launch()
        {
            Task.Factory.StartNew((Action)(() =>
            {
                Process.EnableRaisingEvents = true;
                Process.Exited += this.Process_Exited;

                log.Information("Starting new process");
                Process.Start();
                Pid = Process.Id;

                Notify("Process");

                Process.WaitForInputIdle();
                WindowHandling.ShowWindow(Process.MainWindowHandle, WindowHandling.SW_HIDE);

                SetStatus(Status.Launching);
            }))
            .ContinueWith(t =>
            {
                log.Information("Process {0} is delaying for {1} seconds", Pid, Delay);
                Thread.Sleep(TimeSpan.FromSeconds(Delay));
                SetStatus(Status.Available);
            });
        }

        public event EventHandler Revealed;

        public void Reveal()
        {
            IntPtr hwnd = Process.MainWindowHandle;
            WindowHandling.ShowWindow(hwnd, WindowHandling.SW_SHOWNORMAL);

            log.Information("Process {0} was revealed to user", Pid);

            SetStatus(Status.WithUser);

            if (Revealed != null)
                Revealed(this, new EventArgs());
        }

        private void OnDispose()
        {
            Process.Exited -= Process_Exited;

            switch (status)
            {
                case Status.None:
                    log.Information("Process without pid", Pid);
                    SetStatus(Status.Killed);
                    break;

                case Status.Launching:
                case Status.Available:
                case Status.Reserved:
                    log.Information("Process {0} killed", Pid);
                    Process.Kill();
                    SetStatus(Status.Killed);
                    break;

                case Status.WithUser:
                    // do nothing
                    log.Information("Process {0} - nothing done", Pid);
                    break;
            }
        }
        
        private void Process_Exited(object sender, EventArgs e)
        {
            log.Information("Process {0} has exited", Pid);

            Process.EnableRaisingEvents = false;
            Process.Exited -= Process_Exited;
            Process.Dispose();

            SetStatus(Status.Exited);
            Notify("Process");

            RaiseExited();
        }
        
        internal void SetStatus(Status status)
        {
            if (this.status == status)
                return;

            this.status = status;
            log.Information("Process {0} has new status {1}", Pid, status);
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

        internal Launchable Clone()
        {
            Launchable clone = new Launchable(this.FullPath, this.Arguments, this.DisplayName, this.Delay);
            return clone;
        }

        public event EventHandler HasExited;
        private void RaiseExited()
        {
            if (HasExited != null)
            {
                HasExited(this, new EventArgs());
                log.Debug("Process {0} is raising HasExited event", Pid);
            }
        }

        #region IDisposable

        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    log.Debug("Process {0} is being disposed", Pid);
                    OnDispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                disposed = true;
            }
        }


        ~Launchable()
        {
            Dispose(false);
        }
        #endregion
    }
}

