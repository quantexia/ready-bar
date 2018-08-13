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

namespace Ready.Core
{
    public class Launchable : INotifyPropertyChanged, IDisposable
    {
        public Launchable(string executable, string arguments, string displayName = null, int delay = 0)
        {
            string fullPath = PathHelper.FindExePath(executable);
            Executable = fullPath;

            Arguments = arguments;
            DisplayName = displayName ?? executable;
            Delay = Math.Max(0, delay);

            Process = new Process();
            Process.StartInfo = new ProcessStartInfo(Executable, Arguments) {
                                            WindowStyle = ProcessWindowStyle.Minimized};

            Task.Run(() => ExtractIcon());

            SetStatus(Status.None);
        }

        public string Executable { get; private set; }
        public string Arguments { get; private set; }
        public int Delay { get; private set; }
        public string DisplayName { get; private set; }
        public Process Process { get; private set; }

        public ImageSource Image { get; private set; }

        private void ExtractIcon()
        {
            //Icon icon = Icon.ExtractAssociatedIcon(Process.MainModule.FileName);
            Icon icon = Icon.ExtractAssociatedIcon(Executable);

            Image = 
            System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                new Int32Rect(0, 0, icon.Width, icon.Height),
                BitmapSizeOptions.FromEmptyOptions());

            Notify("Image");
        }

        public void Launch()
        {
            Task.Factory.StartNew((Action)(() =>
            {
                Process.EnableRaisingEvents = true;
                Process.Exited += this.Process_Exited;

                Process.Start();
                Notify("Process");

                Process.WaitForInputIdle();
                WindowHandling.ShowWindow(Process.MainWindowHandle, WindowHandling.SW_HIDE);

                SetStatus(Status.Launching);
            }))
            .ContinueWith(t =>
            {
                Thread.Sleep(Delay * 1000);
                SetStatus(Status.Available);
            });
        }

        public void Reveal()
        {
            IntPtr hwnd = Process.MainWindowHandle;
            WindowHandling.ShowWindow(hwnd, WindowHandling.SW_SHOWNORMAL);
            
            SetStatus(Status.WithUser);
        }

        private void OnDispose()
        {
            Process.Exited -= Process_Exited;
            switch (status)
            {
                case Status.None:
                    SetStatus(Status.Killed);
                    break;

                case Status.Launching:
                case Status.Available:
                case Status.Reserved:
                    Process.Kill();
                    SetStatus(Status.Killed);
                    break;

                case Status.WithUser:
                    // do nothing
                    break;
            }
        }
        
        private void Process_Exited(object sender, EventArgs e)
        {
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
            Launchable clone = new Launchable(this.Executable, this.Arguments, this.DisplayName, this.Delay);
            return clone;
        }

        public event EventHandler HasExited;
        private void RaiseExited()
        {
            if (HasExited != null)
                HasExited(this, new EventArgs());
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

