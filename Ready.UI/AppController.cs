using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ready.Core;
using Serilog;

namespace Ready.UI
{
    public static class AppController
    {
        private static LaunchableMonitor lmon;
        private static ILogger log = Log.ForContext(typeof(AppController));

        public static void Initialize()
        {
            string executable = Configuration.GetValue("Executable");
            string arguments = Configuration.GetValue("Arguments");
            int quantity = Configuration.GetValue<int>("Provision");

            log.Information("Target:\nExecuteable:\t{E}\nArguments:\t{A}\nProvision:\t{Q}", executable, arguments, quantity);

            log.Debug("Init monitor");
            lmon = new LaunchableMonitor();
            lmon.Provision(executable, arguments, quantity);
            log.Debug("Init monitor complete");

            /*
            if (!CheckNotRunning())
                log.Fatal("Program is already running.");
            */
            
        }

        public static LaunchableMonitor Monitor { get { return lmon; } }

        /*
        public static bool CheckNotRunning()
        {
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();

            using (Mutex mutex = new Mutex(false, mutexName))
            {
                if (!mutex.WaitOne(0, true))
                {
                    MessageBox.Show("Unable to run multiple instances of this program.",
                                    "Error",
                                    System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Error);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
            }
        }
        */
    }
}
