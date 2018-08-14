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
        private List<Launchable> coll;
        private Launchable target;
        private const int MIN = 0;
        private const int MAX = 20;

        public LaunchableMonitor()
        {
            coll = new List<Launchable>();
        }

        public Launchable Target { get { return target; } }

        public void Provision(string executable, string arguments, int quantity, string displayName = "", int delay = 0)
        {
            Launchable lb = new Launchable(executable, arguments, displayName, delay);
            this.target = lb;
            SetProvisionLevel(quantity);
        }
        public void Provision(Launchable lb, int quantity)
        {
            this.target = lb;
            SetProvisionLevel(quantity);
        }

        public int ProvisionLevel { get; private set; }

        public void IncrementProvision()
        {
            if (ProvisionLevel == MAX)
                return;

            SetProvisionLevel(ProvisionLevel + 1);
        }
        public void DecrementProvision()
        {
            if (ProvisionLevel == MIN)
                return;

            SetProvisionLevel(ProvisionLevel - 1);
        }
        public void SetProvisionLevel(int quantity)
        {
            if (quantity < MIN || quantity > MAX)
                throw new ArgumentOutOfRangeException("quantity", string.Format("Must be between {0} and {1}", MIN, MAX));

            ProvisionLevel = quantity;

            int cnt = coll.Count(l => l.Status == Status.Available);
            int delta = quantity - cnt;

            switch(Math.Sign(delta))
            { 
                case -1:
                    Enumerable.Range(0, -delta)
                                .ForEach(i => Remove());
                    break;
                case 1:
                    Enumerable.Range(0, delta)
                                .ForEach(i => Add());
                    break;
                case 0:
                    /*nothing*/
                    break;
            }
        }

        private void Add()
        {
            Launchable l = target.Clone();
            l.HasExited += OnLaunchableExit;
            coll.Add(l);
            l.Launch();
        }

        private void Remove()
        {
            Launchable r = NextAvailable;
            if (r != null)
            {
                r.SetStatus(Status.Reserved);
                r.HasExited -= OnLaunchableExit;
                r.Dispose();
                coll.Remove(r);
            }
        }

        private bool shuttingDown = false;
        private void OnDispose()
        {
            coll.ForEach(l =>
            {
                l.Dispose();
                l.HasExited -= OnLaunchableExit;
            });

        }
        
        public IReadOnlyList<Launchable> Launchables { get { return coll; } }

        private void OnLaunchableExit(object sender, EventArgs e)
        {
            Launchable src = (Launchable)sender;
            src.HasExited -= OnLaunchableExit;
            coll.Remove(src);

            if (!shuttingDown)
                Add();
        }

        public bool HasAvailable
        {
            get
            {
                Launchable l;
                return !TryGetAvailable(out l);
            }
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

            return available == null;
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
