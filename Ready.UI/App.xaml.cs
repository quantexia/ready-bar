using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using System.IO;
using System.Windows.Input;

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
            AppController.Initialize(e.Args);

            /*
            string mode = "form";

            log.Debug("Running in mode: {mode}", mode);
            switch (mode)
            {
                case "tray":
                    break;
                case "form":
                    MainWindow win = new MainWindow();
                    App.Current.MainWindow = win;
                    break;
                default:
                    break;
            }
        */
        }
    }
}
