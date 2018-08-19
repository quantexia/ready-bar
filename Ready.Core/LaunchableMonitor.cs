using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ready.Core
{
    public class LaunchableMonitor
    {
        private static ILogger log = Log.Logger.ForContext<LaunchableMonitor>();

        private List<Launchable> coll;
        private Launchable target;
        private const int MIN = 0;
        private const int MAX = 20;
        private const int NO_PID = -1;
        private Func<int> fnCount;

        public LaunchableMonitor()
        {
            log.Debug("c'tor");
            coll = new List<Launchable>();
            fnCount = () => coll.Count(l => l.Status == Status.Available || l.Status == Status.Launching);
        }

        public Launchable Target { get { return target; } }

        public void Provision(string executable, string arguments, int quantity, string displayName = "", int delay = 0, int separation = 0)
        {
            Launchable lb = new Launchable(executable, arguments, displayName, delay, separation);
            this.target = lb;
            ExtractIcon();
            SetProvisionLevel(quantity);
        }

        public void Provision(Launchable lb, int quantity)
        {
            this.target = lb;
            SetProvisionLevel(quantity);
        }

        public int ProvisionLevel { get { return fnCount(); } }

        public void IncrementProvision()
        {
            if (ProvisionLevel == MAX)
                return;

            SetProvisionLevel(ProvisionLevel + 1);
            log.Debug("Provision level is increased to {0}", ProvisionLevel);
        }
        public void DecrementProvision()
        {
            if (ProvisionLevel == MIN)
                return;

            SetProvisionLevel(ProvisionLevel - 1);
            log.Debug("Provision level is decreased to {0}", ProvisionLevel);
        }

        private int targetProvLevel;
        public void SetProvisionLevel(int quantity)
        {
            if (shuttingDown)
                return;
            if (quantity < MIN || quantity > MAX)
                throw new ArgumentOutOfRangeException("quantity", string.Format("Must be between {0} and {1}", MIN, MAX));

            targetProvLevel = quantity;
            //ProvisionLevel = quantity;



            int delta = targetProvLevel - fnCount();

            if (delta == 0)
                return;

            Task t = new Task(() =>
            {
                while (delta != 0)
                {
                    if (Math.Sign(delta) < 0) Remove(); else Add();
                    Thread.Sleep(TimeSpan.FromSeconds(target.Separation));
                    delta -= Math.Sign(delta);
                }
            });
            t.Start();
        }

        private void Add()
        {
            log.Debug("Adding");
            Launchable l = target.Clone();
            l.HasExited += OnRevealedOrExited;
            l.Revealed += OnRevealedOrExited;

            coll.Add(l);
            l.Launch();
        }

        private void Remove()
        {
            Launchable r = NextAvailable;
            if (r != null)
            {
                log.Debug("Removing");
                r.SetStatus(Status.Reserved);
                r.HasExited -= OnRevealedOrExited;
                r.Revealed -= OnRevealedOrExited;
                r.Dispose();
                coll.Remove(r);
            }
        }

        public ImageSource Image { get; private set; }

        private void ExtractIcon()
        {
            log.Debug("Extracing icon");

            Icon icon = Icon.ExtractAssociatedIcon(target.FullPath); 

            Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            new Int32Rect(0, 0, icon.Width, icon.Height),
                            BitmapSizeOptions.FromEmptyOptions());
        }

        private bool shuttingDown = false;
        private void OnDispose()
        {
            shuttingDown = true;
            coll.ForEach(l =>
            {
                l.HasExited -= OnRevealedOrExited;
                l.Revealed -= OnRevealedOrExited;

                l.Dispose();
            });

        }
        
        public IReadOnlyList<Launchable> Launchables { get { return coll; } }

        private void OnRevealedOrExited(object sender, EventArgs e)
        {
            Launchable src = (Launchable)sender;
            src.HasExited -= OnRevealedOrExited;
            coll.Remove(src);

            if (!shuttingDown)
                Add();
        }

        public bool HasAvailable
        {
            get
            {
                Launchable l;
                return TryGetAvailable(out l);
            }
        }

        public int NextPid
        {
            get { Launchable l; return TryGetAvailable(out l) ? l.Pid : NO_PID; }
        }
        public Launchable NextAvailable
        {
            get
            {
                Launchable l;
                TryGetAvailable(out l);
                return l;
            }
        }

        public int CountAvailable
        {
            get
            {
                return coll.Count(l => l.Status == Status.Available);
            }
        }

        private bool TryGetAvailable(out Launchable available)
        {
            if (shuttingDown)
                available = null;
            else 
                available = coll.FirstOrDefault(l => l.Status == Status.Available);

            return available != null;
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


        ~LaunchableMonitor()
        {
            Dispose(false);
        }
#endregion
    }
}
