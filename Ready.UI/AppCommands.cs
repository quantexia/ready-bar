using Ready.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ready.UI
{
    public class AppCommands
    {
        private static RoutedUICommand cmdIncrease;
        private static RoutedUICommand cmdDecrease;
        private static RoutedUICommand cmdExit;

        private static LaunchableMonitor lmon;

        static AppCommands()
        {
            lmon = AppController.Monitor;
            cmdIncrease = new RoutedUICommand("Increment provision", "Increment", typeof(AppCommands));
            cmdDecrease = new RoutedUICommand("Decrement provision", "Decrement", typeof(AppCommands));
            cmdExit = new RoutedUICommand("Exit application", "Exit", typeof(AppCommands)); ;
        }

        public static RoutedUICommand IncrementProvisionCommand { get { return cmdIncrease; } }
        public static RoutedUICommand DecrementProvisionCommand { get { return cmdDecrease; } }
        public static RoutedUICommand ExitCommand { get { return cmdExit; } }
    }
}
