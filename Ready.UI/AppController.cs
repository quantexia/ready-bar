using System;
using System.Collections.Generic;
using System.IO;
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
        private static ILogger log;

        public static void Initialize(string[] mainArgs)
        {
            InitializeLogging();
            InitializeContext(mainArgs);
            InitializeApp();
        }

        private static void InitializeLogging()
        {
            FileInfo fiAsm = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            string logFile = fiAsm.Name.Replace(".exe", ".log");

            Log.Logger = new LoggerConfiguration()
                                    //.WriteTo.File(logFile, rollingInterval: RollingInterval.Infinite)
                                    //.WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                                    .WriteTo.File(logFile)
                                    .MinimumLevel.Debug()
                                    .CreateLogger();

            log = Log.Logger;
            log.Information("\n>>>>>>>>\n\t>>>>>>>>\n\t\t>>>>>>>>\n\t>>>>>>>>\n>>>>>>>>");
        }

        private static void InitializeContext(string[] args)
        {
            log.Debug("Args were passed ({0}): {1}", string.Join(" ", args.Length, args.Select(a => string.Format("'{0}'", a))));

            if (args.Length > 0)
            {
                log.Information("Configuring from command line arguments");
                Configuration.FromCommandLine(args);
            }
            else
            {
                log.Information("Configuring appSettings");
                Configuration.FromAppSettings();
            }
        }

        private static void InitializeApp()
        {
            string executable = Configuration.GetValue("Executable");
            string arguments = Configuration.GetValue("Arguments");
            int quantity = Configuration.GetValue<int>("Provision");

            log.Information("Target:\nExecuteable:\t{E}\nArguments:\t{A}\nProvision:\t{Q}", executable, arguments, quantity);
            log.Debug("Initializing monitor");
            lmon = new LaunchableMonitor();
            lmon.Provision(executable, arguments, quantity);
            log.Information("Initialization of monitor complete");

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
