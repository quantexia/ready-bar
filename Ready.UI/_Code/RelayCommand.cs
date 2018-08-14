﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ready.UI
{
    public class RelayCommand<T> : ICommand
    {
        #region Fields 
        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;
        #endregion // Fields 

        #region Constructors 
        public RelayCommand(Action<T> execute) 
            : this(execute, null)
        { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 

        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        //public void Execute(T parameter) { _execute(parameter); }
        public void Execute(object parameter) { _execute((T)parameter); }
        #endregion // ICommand Members 
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute)
            : this(o => execute(), o => true)
        { }
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        { }
    }
}
