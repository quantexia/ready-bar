using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using System.IO;

namespace Ready.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ILogger log = Log.ForContext(typeof(App));

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FileInfo fiAsm = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            string logFile = fiAsm.Name.Replace(".exe", ".log");

            Log.Logger = new LoggerConfiguration()
                                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                                .CreateLogger();
            Log.Information("\n\n\n\n>>>>>>>>\n\t>>>>>>>>\n\t\t>>>>>>>>\n\t>>>>>>>>\n>>>>>>>>");

            if (e.Args.Length > 0)
                Configuration.FromCommandLine(e.Args);
            else
                Configuration.FromAppSettings();

            AppController.Initialize();

            string mode = "form";

            log.Debug("Running in mode: {mode}", mode);
            switch (mode)
            {
                case "tray":
                    break;
                case "form":
                    MainWindow win = new MainWindow();
                    win.ShowDialog();
                    break;
                default:
                    break;
            }
        }
    }
}
