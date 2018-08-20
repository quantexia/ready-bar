using Ready.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ready.UI
{
    public class AppCommands
    {
        private static RoutedUICommand cmdIncrease;
        private static RoutedUICommand cmdDecrease;
        private static RoutedUICommand cmdReveal;
        private static RoutedUICommand cmdExit;

        private static LaunchableMonitor lmon;

        static AppCommands()
        {
            lmon = AppController.Monitor;

            cmdIncrease = new RoutedUICommand("Increment provision", "Increment", typeof(AppCommands));
            cmdDecrease = new RoutedUICommand("Decrement provision", "Decrement", typeof(AppCommands));
            cmdReveal = new RoutedUICommand("Reveal next instance", "Reveal", typeof(AppCommands));
            cmdExit = new RoutedUICommand("Exit application", "Exit", typeof(AppCommands)); ;

            var bind1 = new CommandBinding(AppCommands.IncrementProvisionCommand, Execute, CanExecute);
            var bind2 = new CommandBinding(AppCommands.DecrementProvisionCommand, Execute, CanExecute);
            var bind3 = new CommandBinding(AppCommands.RevealCommand, Execute, CanExecute);
            var bind4 = new CommandBinding(AppCommands.ExitCommand, Execute, CanExecute);

            CommandManager.RegisterClassCommandBinding(typeof(Window), bind1);
            CommandManager.RegisterClassCommandBinding(typeof(Window), bind2);
            CommandManager.RegisterClassCommandBinding(typeof(Window), bind3);
            CommandManager.RegisterClassCommandBinding(typeof(Window), bind4);
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedUICommand ruic = (RoutedUICommand)e.Command;

            switch (ruic.Name)
            {
                case "Increment":
                    lmon.IncrementProvision();
                    break;
                case "Decrement":
                    lmon.DecrementProvision();
                    break;
                case "Reveal":
                    lmon.RevealNext();
                    break;
                case "Exit":
                    lmon.Dispose();
                    break;
            }
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool canExecute = false;

            RoutedUICommand ruic = (RoutedUICommand)e.Command;

            switch (ruic.Name)
            {
                case "Increment":
                    canExecute = lmon.ProvisionLevel <= LaunchableMonitor.PROVISIONLEVEL_MAX;
                    break;
                case "Decrement":
                    canExecute = lmon.ProvisionLevel > 0;
                    break;
                case "Reveal":
                    canExecute = lmon.HasAvailable;
                    break;
                case "Exit":
                    canExecute = true;
                    break;
            }
            e.CanExecute = canExecute;
        }

        public static RoutedUICommand IncrementProvisionCommand { get { return cmdIncrease; } }
        public static RoutedUICommand DecrementProvisionCommand { get { return cmdDecrease; } }
        public static RoutedUICommand RevealCommand { get { return cmdReveal; } }
        public static RoutedUICommand ExitCommand { get { return cmdExit; } }
    }
}
