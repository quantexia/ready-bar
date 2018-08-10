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
    public class Launchable : INotifyPropertyChanged
    {
        public Launchable(string executable, string arguments, string displayName = null, int delay = 0)
        {
            Executable = executable;
            Arguments = arguments;
            DisplayName = displayName ?? executable;
            Delay = Math.Max(0, delay);

            Process = new Process();
            Process.StartInfo = new ProcessStartInfo(Executable, Arguments) { WindowStyle = ProcessWindowStyle.Minimized };
            
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
            Icon ico = Icon.ExtractAssociatedIcon(Process.MainModule.FileName);

            Bitmap bitmap = ico.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap =
                 Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions());

            Image = wpfBitmap;
        }

        public void Launch()
        {
            Task.Factory.StartNew(() =>
            {
                Process.EnableRaisingEvents = true;
                Process.Exited += Process_Exited;

                Process.Start();
                Notify("Process");

                Process.WaitForInputIdle();
                WindowHandling.ShowWindow(Process.MainWindowHandle, WindowHandling.SW_HIDE);

                SetStatus(Status.Launching);
            })
            .ContinueWith(t =>
            {
                Thread.Sleep(Delay * 1000);
                SetStatus(Status.Available);
            })
            .ContinueWith(t =>
            {
                //ExtractIcon();
            });
        }

        public void Reveal()
        {
            //Process.WaitForInputIdle();
            IntPtr hwnd = Process.MainWindowHandle;
            WindowHandling.ShowWindow(hwnd, WindowHandling.SW_SHOWNORMAL);
            
            SetStatus(Status.WithUser);
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
    }
}

